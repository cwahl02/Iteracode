// using Iteracode.Api.Abstractions;
// using Iteracode.Api.Services;
// using Microsoft.AspNetCore.Mvc;

// namespace Iteracode.Api.Features.Playground;

// public sealed class Run : IEndpoint
// {
//     public sealed record PlaygroundRunRequest(
//         string Code,
//         string Language,
//         string? Stdin = null);

//     public sealed record RunResponse(
//         string? Stdout,
//         string? Stderr,
//         string? CompileOutput,
//         int     StatusId,
//         string  StatusDescription);

//     public void MapEndpoint(IEndpointRouteBuilder app)
//     {
//         app.MapPost("/api/playground/run", Handle)
//             .WithTags("Playground")
//             .WithName("PlaygroundRun")
//             .WithSummary("Runs arbitrary code in the playground.")
//             .Produces<RunResponse>(StatusCodes.Status200OK)
//             .Produces(StatusCodes.Status400BadRequest)
//             .RequireAuthorization();
//     }

//     public static async Task<IResult> Handle(
//         [FromServices] IJudge0Client   judge0,
//         [FromServices] ILanguageService languageService,
//         [FromBody]     PlaygroundRunRequest       request,
//         CancellationToken ct)
//     {
//         int languageId;
//         try
//         {
//             languageId = await languageService.GetJudge0IdAsync(request.Language, ct);
//         }
//         catch (KeyNotFoundException ex)
//         {
//             return Results.BadRequest(new { Error = ex.Message });
//         }

//         var result = await judge0.RunAsync(request.Code, languageId, request.Stdin, ct);

//         return Results.Ok(new RunResponse(
//             result.Stdout,
//             result.Stderr,
//             result.CompileOutput,
//             result.Status.Id,
//             result.Status.Description));
//     }
// }