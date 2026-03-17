using Iteracode.Api.Abstractions;
using Iteracode.Api.Data;
using Iteracode.Api.Services;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using Iteracode.Api.Features.Fs;
using Microsoft.AspNetCore.Mvc;

namespace Iteracode.Api.Features.Problems;

public sealed class GetProblem : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/problems/{slug}", Handle)
            .WithTags("Problems")
            .Produces<ProblemManifest>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound);
    }

    public static async Task<IResult> Handle(
        string slug,
        [FromServices] ApplicationDbContext db,
        [FromServices] IVfsService vfs,
        CancellationToken ct)
    {
        var problem = await db.Problems
            .FirstOrDefaultAsync(p => p.Slug == slug && p.Published && p.DeletedAt == null, ct);

        if (problem is null) return Results.NotFound();

        var jsonPath = vfs.Resolve($"root/problems/{slug}/problem.json");
        if (!File.Exists(jsonPath)) return Results.NotFound();

        var json     = await File.ReadAllTextAsync(jsonPath, ct);
        var manifest = JsonSerializer.Deserialize<ProblemManifest>(json);
        return manifest is null ? Results.NotFound() : Results.Ok(manifest);
    }
}