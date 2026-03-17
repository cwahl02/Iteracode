using Iteracode.Api.Abstractions;
using Iteracode.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace Iteracode.Api.Features.Fs;

public sealed class DeleteFolder : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapDelete("/api/fs/folder", Handle)
            .WithTags("FS")
            .RequireAuthorization()
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status404NotFound);
    }

    public static IResult Handle(
        string path,
        [FromServices] IVfsService vfs)
    {
        if (vfs.IsProtected(path))
            return Results.BadRequest(new { Errors = new[] { "Path is protected." } });

        var absolute = vfs.Resolve(path);
        if (!Directory.Exists(absolute))
            return Results.NotFound();

        Directory.Delete(absolute, recursive: true);
        return Results.NoContent();
    }
}