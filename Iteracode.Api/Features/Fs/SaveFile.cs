using Iteracode.Api.Abstractions;
using Iteracode.Api.Data;
using Iteracode.Api.Services;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using Iteracode.Api.Models;
using Microsoft.AspNetCore.Mvc;

namespace Iteracode.Api.Features.Fs;

public sealed class SaveFile : IEndpoint
{
    public sealed record SaveFileRequest(string Path, string Content);

    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPut("/api/fs/file", Handle)
            .WithTags("FS")
            .RequireAuthorization()
            .Produces(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status400BadRequest);
    }

    public static async Task<IResult> Handle(
        [FromBody] SaveFileRequest request,
        [FromServices] IVfsService vfs,
        [FromServices] ApplicationDbContext db,
        CancellationToken ct)
    {
        if (vfs.IsProtected(request.Path))
            return Results.BadRequest(new { Errors = new[] { "Path is protected." } });

        var absolute = vfs.Resolve(request.Path);
        var dir = System.IO.Path.GetDirectoryName(absolute)!;
        if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);

        await File.WriteAllTextAsync(absolute, request.Content, ct);

        // If saving problem.json → upsert DB row
        if (vfs.IsProblemJson(request.Path))
            await UpsertProblemAsync(request.Content, db, ct);

        return Results.Ok();
    }

    private static async Task UpsertProblemAsync(
        string json, ApplicationDbContext db, CancellationToken ct)
    {
        try
        {
            var manifest = JsonSerializer.Deserialize<ProblemManifest>(json);
            if (manifest is null || string.IsNullOrWhiteSpace(manifest.Slug)) return;

            var existing = await db.Problems
                .FirstOrDefaultAsync(p => p.Slug == manifest.Slug, ct);

            if (existing is null)
            {
                db.Problems.Add(new Problem
                {
                    Slug      = manifest.Slug,
                    Published = manifest.Published,
                    CreatedAt = DateTimeOffset.UtcNow,
                    UpdatedAt = DateTimeOffset.UtcNow,
                });
            }
            else
            {
                existing.Published = manifest.Published;
                existing.UpdatedAt = DateTimeOffset.UtcNow;
                existing.DeletedAt = null; // restore if previously soft-deleted
            }

            await db.SaveChangesAsync(ct);
        }
        catch (JsonException) { /* malformed json — skip upsert */ }
    }
}