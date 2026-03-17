using Iteracode.Api.Abstractions;
using Iteracode.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace Iteracode.Api.Features.Fs;

public sealed class RenameNode : IEndpoint
{
    public sealed record RenameRequest(string Path, string NewName);

    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("/api/fs/rename", Handle)
            .WithTags("FS")
            .RequireAuthorization()
            .Produces<object>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status404NotFound);
    }

    public static IResult Handle(
        [FromBody] RenameRequest request,
        [FromServices] IVfsService vfs)
    {
        if (vfs.IsProtected(request.Path))
            return Results.BadRequest(new { Errors = new[] { "Path is protected." } });
        if (request.NewName.Contains('/') || request.NewName.Contains('\\'))
            return Results.BadRequest(new { Errors = new[] { "Name cannot contain path separators." } });

        var sourceAbs  = vfs.Resolve(request.Path);
        var parts      = request.Path.Split('/');
        parts[^1]      = request.NewName;
        var newVfsPath = string.Join('/', parts);
        var targetAbs  = vfs.Resolve(newVfsPath);

        if (!Directory.Exists(sourceAbs) && !File.Exists(sourceAbs))
            return Results.NotFound();

        if (Directory.Exists(sourceAbs)) Directory.Move(sourceAbs, targetAbs);
        else File.Move(sourceAbs, targetAbs);

        return Results.Ok(new { newPath = newVfsPath });
    }
}