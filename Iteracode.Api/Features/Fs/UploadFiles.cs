using Iteracode.Api.Abstractions;
using Iteracode.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace Iteracode.Api.Features.Fs;

public sealed class UploadFiles : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("/api/fs/upload", Handle)
            .WithTags("FS")
            .RequireAuthorization()
            .DisableAntiforgery()
            .Produces(StatusCodes.Status201Created)
            .Produces(StatusCodes.Status400BadRequest);
    }

    public static async Task<IResult> Handle(
        IFormFileCollection files,
        string parentPath,
        [FromServices] IVfsService vfs,
        CancellationToken ct)
    {
        if (vfs.IsProtected(parentPath))
            return Results.BadRequest(new { Errors = new[] { "Path is protected." } });

        var parentAbs = vfs.Resolve(parentPath);
        if (!Directory.Exists(parentAbs)) Directory.CreateDirectory(parentAbs);

        foreach (var file in files)
        {
            var destAbs = Path.Combine(parentAbs, file.FileName);
            await using var stream = File.Create(destAbs);
            await file.CopyToAsync(stream, ct);
        }

        return Results.Created();
    }
}