using System.Security.Claims;
using Iteracode.Api.Abstractions;
using Iteracode.Api.Data;
using Iteracode.Api.Models;
using Microsoft.AspNetCore.Mvc;

namespace Iteracode.Api.Features.Submissions;

public sealed class Submit : IEndpoint
{
    public sealed record SubmitRequest(string ProblemSlug, string Language, bool Passed);
    public sealed record SubmitResponse(bool Passed, DateTimeOffset SubmittedAt);

    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("/api/submissions", Handle)
            .WithTags("Submissions")
            .RequireAuthorization()
            .Produces<SubmitResponse>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status400BadRequest);
    }

    public static async Task<IResult> Handle(
        [FromBody] SubmitRequest request,
        HttpContext httpContext,
        [FromServices] ApplicationDbContext db,
        CancellationToken ct)
    {
        var userId = httpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId))
            return Results.BadRequest(new { Errors = new[] { "User not found." } });

        var submission = new Submission
        {
            UserId      = userId,
            ProblemSlug = request.ProblemSlug,
            Language    = request.Language,
            Passed      = request.Passed,
            SubmittedAt = DateTimeOffset.UtcNow,
        };

        db.Submissions.Add(submission);
        await db.SaveChangesAsync(ct);

        return Results.Ok(new SubmitResponse(submission.Passed, submission.SubmittedAt));
    }
}