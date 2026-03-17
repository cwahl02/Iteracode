using Iteracode.Api.Abstractions;
using Iteracode.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace Iteracode.Api.Features.Fs;

public sealed class CreateFolder : IEndpoint
{
    public sealed record CreateFolderRequest(string Path);

    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("/api/fs/folder", Handle)
            .WithTags("FS")
            .RequireAuthorization()
            .Produces(StatusCodes.Status201Created)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status409Conflict);
    }

    public static IResult Handle(
        [FromBody] CreateFolderRequest request,
        [FromServices] IVfsService vfs)
    {
        if (vfs.IsProtected(request.Path))
            return Results.BadRequest(new { Errors = new[] { "Path is protected." } });

        var absolute = vfs.Resolve(request.Path);
        if (Directory.Exists(absolute))
            return Results.Conflict(new { Errors = new[] { "Folder already exists." } });

        Directory.CreateDirectory(absolute);
        return Results.Created();
    }
}