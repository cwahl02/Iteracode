using System.Security.Claims;
using Iteracode.Api.Abstractions;
using Iteracode.Api.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;

namespace Iteracode.Api.Features.Submissions;

public sealed class GetSubmission : IEndpoint
{
    public sealed record SubmissionDto(bool Passed, string Language, DateTimeOffset SubmittedAt);

    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/submissions/{slug}", Handle)
            .WithTags("Submissions")
            .RequireAuthorization()
            .Produces<SubmissionDto>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound);
    }

    public static async Task<IResult> Handle(
        string slug,
        HttpContext httpContext,
        [FromServices] ApplicationDbContext db,
        CancellationToken ct)
    {
        var userId = httpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId)) return Results.NotFound();

        var submission = await db.Submissions
            .Where(s => s.UserId == userId && s.ProblemSlug == slug)
            .OrderByDescending(s => s.SubmittedAt)
            .FirstOrDefaultAsync(ct);

        if (submission is null) return Results.NotFound();

        return Results.Ok(new SubmissionDto(
            submission.Passed,
            submission.Language,
            submission.SubmittedAt
        ));
    }
}