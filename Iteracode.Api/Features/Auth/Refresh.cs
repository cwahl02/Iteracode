using FluentValidation;
using Iteracode.Api.Abstractions;
using Iteracode.Api.Data;
using Iteracode.Api.Models;
using Iteracode.Api.Options;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace Iteracode.Api.Features.Auth;

public sealed class Refresh : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("/api/auth/refresh", Handle)
            .WithTags("Auth")
            .WithName("Refresh")
            .WithSummary("Refreshes the access token.")
            .WithDescription("Generates a new access token using the provided refresh token.")
            .Produces(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status400BadRequest);
    }

    public static async Task<IResult> Handle(
        HttpContext httpContext,
        [FromServices] ApplicationDbContext dbContext,
        [FromServices] IOptions<JwtOptions> jwtOptions,
        [FromServices] SignInManager<User> signInManager,
        [FromServices] ITokenService tokenService,
        [FromServices] IHashingService hashingService,
        CancellationToken cancellationToken)
    {
        var refreshToken = httpContext.Request.Cookies["refreshToken"];
        if (string.IsNullOrEmpty(refreshToken))
            return Results.BadRequest(new { Errors = new[] { "Refresh token is required." } });

        var hashedToken = hashingService.Hash(refreshToken);

        var dbRefreshToken = await dbContext.RefreshTokens
            .FirstOrDefaultAsync(rt => rt.TokenHash == hashedToken, cancellationToken);

        if (dbRefreshToken == null)
        {
            return Results.BadRequest(new { Errors = new[] { "Invalid refresh token." } });
        }

        // Check expiry
        if (dbRefreshToken.Expiry <= DateTimeOffset.UtcNow)
        {
            dbContext.RefreshTokens.Remove(dbRefreshToken);
            await dbContext.SaveChangesAsync(cancellationToken);
            return Results.BadRequest(new { Errors = new[] { "Refresh token expired." } });
        }

        var user = await signInManager.UserManager.FindByIdAsync(dbRefreshToken.UserId);
        if (user == null)
            return Results.BadRequest(new { Errors = new[] { "Invalid refresh token." } });

        // Generate new tokens
        var accessToken = tokenService.GenerateAccessToken(user);
        var (newRefreshTokenRaw, newRefreshTokenHashed) = tokenService.GenerateRefreshToken();

        // Start a transaction for atomic rotation
        using var transaction = await dbContext.Database.BeginTransactionAsync(cancellationToken);

        // Remove old refresh token
        dbContext.RefreshTokens.Remove(dbRefreshToken);

        // Add new refresh token
        dbContext.RefreshTokens.Add(new RefreshToken
        {
            UserId = user.Id,
            TokenHash = newRefreshTokenHashed,
            Expiry = DateTimeOffset.UtcNow.AddDays(jwtOptions.Value.RefreshTokenExpiryDays),
            DeviceInfo = httpContext.Request.Headers["User-Agent"].ToString(),
            CreatedAt = DateTimeOffset.UtcNow
        });

        await dbContext.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);

        // Send new refresh token in HttpOnly cookie
        httpContext.Response.Cookies.Append("refreshToken", newRefreshTokenRaw, new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.Strict,
            Path = "/",
            Expires = DateTimeOffset.UtcNow.AddDays(jwtOptions.Value.RefreshTokenExpiryDays)
        });

        return Results.Ok(new { AccessToken = accessToken });
    }
}