using FluentValidation;
using Iteracode.Api.Abstractions;
using Iteracode.Api.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Iteracode.Api.Features.Auth;

public sealed class Register : IEndpoint
{
    public sealed record Request(string Email, string Username, string Password, string ConfirmPassword);

    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("/api/auth/register", Handle)
            .WithTags("Auth")
            .WithName("Register")
            .WithSummary("Registers a new user.")
            .WithDescription("Creates a new user account with the provided email, username, and password.")
            .Produces(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status400BadRequest);
    }

    public static async Task<IResult> Handle(
        [FromServices] UserManager<User> userManager,
        [FromBody] Request request,
        CancellationToken cancellationToken)
    {
        var newUser = new User
        {
            Email = request.Email,
            UserName = request.Username
        };

        var userValidation = await userManager.UserValidators[0].ValidateAsync(userManager, newUser);
        var passwordValidation = await userManager.PasswordValidators[0].ValidateAsync(userManager, newUser, request.Password);

        var registerResult = await userManager.CreateAsync(newUser, request.Password);

        if (!registerResult.Succeeded)
        {
            var registerErrors = new List<string>();
            registerErrors.AddRange(userValidation.Errors.Select(e => e.Description));
            registerErrors.AddRange(passwordValidation.Errors.Select(e => e.Description));
            return Results.BadRequest(new { Errors = registerErrors });
        }

        return Results.Created();
    }
}