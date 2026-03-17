using Iteracode.Api.Abstractions;
using Iteracode.Api.Data;
using Iteracode.Api.Services;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using Iteracode.Api.Features.Fs;
using Microsoft.AspNetCore.Mvc;

namespace Iteracode.Api.Features.Problems;

public sealed class GetProblems : IEndpoint
{
    public record ProblemSummaryDto(
        string Slug, bool Published, string Title,
        List<string> Tags, List<string> Languages, DateTimeOffset CreatedAt);

    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/problems", Handle)
            .WithTags("Problems")
            .Produces<List<ProblemSummaryDto>>(StatusCodes.Status200OK);
    }

    public static async Task<IResult> Handle(
        [FromServices] ApplicationDbContext db,
        [FromServices] IVfsService vfs,
        CancellationToken ct)
    {
        var problems = await db.Problems
            .Where(p => p.Published && p.DeletedAt == null)
            .OrderBy(p => p.CreatedAt)
            .ToListAsync(ct);

        var result = new List<ProblemSummaryDto>();

        foreach (var p in problems)
        {
            var jsonPath = vfs.Resolve($"root/problems/{p.Slug}/problem.json");
            if (!File.Exists(jsonPath)) continue;

            try
            {
                var json     = await File.ReadAllTextAsync(jsonPath, ct);
                var manifest = JsonSerializer.Deserialize<ProblemManifest>(json);
                if (manifest is null) continue;

                result.Add(new ProblemSummaryDto(
                    Slug:      p.Slug,
                    Published: p.Published,
                    Title:     manifest.Title,
                    Tags:      manifest.Tags,
                    Languages: manifest.Languages.Keys.ToList(),
                    CreatedAt: p.CreatedAt
                ));
            }
            catch { /* skip malformed manifests */ }
        }

        return Results.Ok(result);
    }
}