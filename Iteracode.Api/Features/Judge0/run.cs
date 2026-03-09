using FluentValidation;
using Iteracode.Api.Abstractions;
using Iteracode.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace Iteracode.Api.Features.Judge0;

public sealed class Run : IEndpoint
{
    public sealed record RunRequest(string Code, int LanguageId, string? Stdin = null);

    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("/api/judge0/run", Handle)
            .WithTags("Judge0")
            .WithName("Run")
            .WithSummary("Runs code using Judge0.")
            .WithDescription("Executes the provided code with the specified language and optional input.")
            .Produces(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status400BadRequest);
    }

    public static async Task<IResult> Handle(
        [FromServices] IJudge0Service judge0Service,
        [FromBody] RunRequest request,
        CancellationToken cancellationToken)
    {
        var result = await judge0Service.RunAsync(request.Code, request.LanguageId, request.Stdin);
        return Results.Ok(result);
    }
}