using Iteracode.Api.Abstractions;
using Iteracode.Api.Data;
using Iteracode.Api.Options;
using Iteracode.Api.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace Iteracode.Api.Features.Fs;

public sealed class DeleteFile : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapDelete("/api/fs/file", Handle)
            .WithTags("FS")
            .RequireAuthorization()
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status404NotFound);
    }

    public static async Task<IResult> Handle(
        string path,
        [FromServices] IVfsService vfs,
        [FromServices] ApplicationDbContext db,
        [FromServices] IOptions<VfsOptions> options,
        CancellationToken ct)
    {
        if (vfs.IsProtected(path))
            return Results.BadRequest(new { Errors = new[] { "Path is protected." } });

        var absolute = vfs.Resolve(path);
        if (!File.Exists(absolute))
            return Results.NotFound();

        // If deleting problem.json → soft delete DB row + recycle parent folder
        if (vfs.IsProblemJson(path))
        {
            var slug = vfs.GetParentProblemSlug(path);
            if (!string.IsNullOrEmpty(slug))
            {
                var problem = await db.Problems
                    .FirstOrDefaultAsync(p => p.Slug == slug, ct);
                if (problem is not null)
                {
                    problem.DeletedAt = DateTimeOffset.UtcNow;
                    await db.SaveChangesAsync(ct);
                }

                // Move parent folder to recycled/
                var parentVfsPath = string.Join('/', path.Split('/')[..^1]);
                var recyclePath   = vfs.BuildRecyclePath(slug);
                var parentAbs     = vfs.Resolve(parentVfsPath);
                var recycleAbs    = vfs.Resolve(recyclePath);

                var recycleDir = System.IO.Path.GetDirectoryName(recycleAbs)!;
                if (!Directory.Exists(recycleDir)) Directory.CreateDirectory(recycleDir);

                Directory.Move(parentAbs, recycleAbs);
                return Results.NoContent();
            }
        }

        File.Delete(absolute);
        return Results.NoContent();
    }
}