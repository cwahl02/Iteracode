using Iteracode.Api.Abstractions;
using Iteracode.Api.Data;
using Iteracode.Api.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using Iteracode.Api.Models;

namespace Iteracode.Api.Features.Fs;

public sealed class MoveNode : IEndpoint
{
    public sealed record MoveRequest(string SourcePath, string TargetPath);

    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("/api/fs/move", Handle)
            .WithTags("FS")
            .RequireAuthorization()
            .Produces(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status404NotFound);
    }

    public static async Task<IResult> Handle(
        [FromBody] MoveRequest request,
        [FromServices] IVfsService vfs,
        [FromServices] ApplicationDbContext db,
        CancellationToken ct)
    {
        if (vfs.IsProtected(request.SourcePath))
            return Results.BadRequest(new { Errors = new[] { "Source path is protected." } });
        if (vfs.IsProtected(request.TargetPath))
            return Results.BadRequest(new { Errors = new[] { "Target path is protected." } });

        var sourceAbs = vfs.Resolve(request.SourcePath);
        var targetAbs = vfs.Resolve(request.TargetPath);

        if (!Directory.Exists(sourceAbs) && !File.Exists(sourceAbs))
            return Results.NotFound();

        var targetDir = System.IO.Path.GetDirectoryName(targetAbs)!;
        if (!Directory.Exists(targetDir)) Directory.CreateDirectory(targetDir);

        var isDir = Directory.Exists(sourceAbs);
        if (isDir) Directory.Move(sourceAbs, targetAbs);
        else File.Move(sourceAbs, targetAbs);

        // recycled/ → root/problems/ restore
        var recycledSlug = vfs.TryExtractRecycledSlug(request.SourcePath);
        if (recycledSlug is not null && request.TargetPath.StartsWith("root/problems/"))
        {
            await RestoreProblemAsync(request.TargetPath, recycledSlug, db, ct);
        }

        // root/problems/ → root/recycled/ soft delete
        if (request.SourcePath.StartsWith("root/problems/") &&
            request.TargetPath.StartsWith("root/recycled/"))
        {
            var slug = request.SourcePath.Split('/').ElementAtOrDefault(2);
            if (!string.IsNullOrEmpty(slug))
            {
                var problem = await db.Problems.FirstOrDefaultAsync(p => p.Slug == slug, ct);
                if (problem is not null)
                {
                    problem.DeletedAt = DateTimeOffset.UtcNow;
                    await db.SaveChangesAsync(ct);
                }
            }
        }

        return Results.Ok();
    }

    private static async Task RestoreProblemAsync(
        string targetVfsPath, string slug,
        ApplicationDbContext db, CancellationToken ct)
    {
        try
        {
            var jsonPath = System.IO.Path.Combine(
                targetVfsPath.Replace('/', System.IO.Path.DirectorySeparatorChar),
                "problem.json");

            if (!File.Exists(jsonPath)) return;
            var json = await File.ReadAllTextAsync(jsonPath, ct);
            var manifest = JsonSerializer.Deserialize<ProblemManifest>(json);
            if (manifest is null) return;

            var existing = await db.Problems.FirstOrDefaultAsync(p => p.Slug == slug, ct);
            if (existing is not null)
            {
                existing.Published = manifest.Published;
                existing.DeletedAt = null;
                existing.UpdatedAt = DateTimeOffset.UtcNow;
            }
            else
            {
                db.Problems.Add(new Problem
                {
                    Slug      = manifest.Slug,
                    Published = manifest.Published,
                    CreatedAt = DateTimeOffset.UtcNow,
                    UpdatedAt = DateTimeOffset.UtcNow,
                });
            }
            await db.SaveChangesAsync(ct);
        }
        catch { /* if json is malformed just skip */ }
    }
}