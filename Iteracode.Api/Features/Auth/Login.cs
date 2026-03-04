using FluentValidation;
using Iteracode.Api.Abstractions;
using Iteracode.Api.Data;
using Iteracode.Api.Models;
using Iteracode.Api.Options;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace Iteracode.Api.Features.Auth;

public sealed class Login : IEndpoint
{
    public sealed record Request(string EmailOrUsername, string Password);

    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("/api/auth/login", Handle)
            .WithTags("Auth")
            .WithName("Login")
            .WithSummary("Logs in a user.")
            .WithDescription("Authenticates a user with the provided email/username and password.")
            .Produces(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status400BadRequest);
    }

    public static async Task<IResult> Handle(
        [FromServices] ApplicationDbContext dbContext,
        [FromServices] IOptions<JwtOptions> jwtOptions,
        HttpContext httpContext,
        [FromServices] SignInManager<User> signInManager,
        [FromServices] ITokenService tokenService,
        [FromBody] Request request,
        CancellationToken cancellationToken)
    {
        var user = await signInManager.UserManager.FindByEmailAsync(request.EmailOrUsername)
            ?? await signInManager.UserManager.FindByNameAsync(request.EmailOrUsername);

        if (user == null)
            return Results.BadRequest(new { Errors = new[] { "Invalid email/username or password." } });

        var signInResult = await signInManager.CheckPasswordSignInAsync(user, request.Password, false);

        if (!signInResult.Succeeded)
            return Results.BadRequest(new { Errors = new[] { "Invalid email/username or password." } });


        var accessToken = tokenService.GenerateAccessToken(user);
        var (refreshTokenRaw, refreshTokenHashed) = tokenService.GenerateRefreshToken();

        // Store refresh token in HttpOnly cookie
        httpContext.Response.Cookies.Append("refreshToken", refreshTokenRaw, new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.Strict,
            Expires = DateTimeOffset.UtcNow.AddDays(jwtOptions.Value.RefreshTokenExpiryDays)
        });

        // Store hashed refresh token in database with user association and expiry
        dbContext.RefreshTokens.Add(new RefreshToken
        {
            UserId = user.Id,
            TokenHash = refreshTokenHashed,
            Expiry = DateTimeOffset.UtcNow.AddDays(jwtOptions.Value.RefreshTokenExpiryDays),
            DeviceInfo = httpContext.Request.Headers["User-Agent"].ToString(),
            CreatedAt = DateTimeOffset.UtcNow
        });

        await dbContext.SaveChangesAsync(); 

        // Return access token in response body
        return Results.Ok(new { AccessToken = accessToken });
    }
}