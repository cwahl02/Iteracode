using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
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

public sealed class Logout : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("/api/auth/logout", Handle)
            .WithTags("Auth")
            .WithName("Logout")
            .WithSummary("Logs out a user.")
            .WithDescription("Revokes the user's access and refresh tokens.")
            .RequireAuthorization()
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status400BadRequest);
    }

    public static async Task<IResult> Handle(
        HttpContext httpContext,
        [FromServices] ApplicationDbContext dbContext,
        [FromServices] IBlacklistService blacklistService,
        [FromServices] IHashingService hashingService,
        [FromServices] IOptions<JwtOptions> jwtOptions,
        CancellationToken cancellationToken)
    {
        // Blacklist the current access token
        var jti = httpContext.User.FindFirstValue(JwtRegisteredClaimNames.Jti);
        var exp = httpContext.User.FindFirstValue(JwtRegisteredClaimNames.Exp);

        if (string.IsNullOrEmpty(jti) || string.IsNullOrEmpty(exp) || !long.TryParse(exp, out var expSeconds))
            return Results.BadRequest(new { Errors = new[] { "Invalid token." } });

        var expiry = DateTimeOffset.FromUnixTimeSeconds(expSeconds);
        await blacklistService.RevokeAsync(jti, expiry);

        // Revoke the refresh token row in the DB
        var rawRefreshToken = httpContext.Request.Cookies["refreshToken"];
        if (!string.IsNullOrEmpty(rawRefreshToken))
        {
            var tokenHash = hashingService.Hash(rawRefreshToken);
            var refreshToken = await dbContext.RefreshTokens.FirstOrDefaultAsync(rt => rt.TokenHash == tokenHash && rt.RevokedAt == null, cancellationToken);
                //.FirstOrDefaultAsync(t => t.TokenHash == tokenHash && t.RevokedAt == null, cancellationToken);

            if (refreshToken is not null)
            {
                refreshToken.RevokedAt = DateTimeOffset.UtcNow;
                await dbContext.SaveChangesAsync(cancellationToken);
            }
        }

        httpContext.Response.Cookies.Delete("refreshToken");

        return Results.NoContent();
    }
}