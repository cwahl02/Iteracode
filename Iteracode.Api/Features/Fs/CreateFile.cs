using Iteracode.Api.Abstractions;
using Iteracode.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace Iteracode.Api.Features.Fs;

public sealed class CreateFile : IEndpoint
{
    public sealed record CreateFileRequest(string Path, string? Content);

    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("/api/fs/file", Handle)
            .WithTags("FS")
            .RequireAuthorization()
            .Produces(StatusCodes.Status201Created)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status409Conflict);
    }

    public static async Task<IResult> Handle(
        [FromBody] CreateFileRequest request,
        [FromServices] IVfsService vfs,
        CancellationToken ct)
    {
        if (vfs.IsProtected(request.Path))
            return Results.BadRequest(new { Errors = new[] { "Path is protected." } });

        var absolute = vfs.Resolve(request.Path);
        if (File.Exists(absolute))
            return Results.Conflict(new { Errors = new[] { "File already exists." } });

        var dir = System.IO.Path.GetDirectoryName(absolute)!;
        if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);

        await File.WriteAllTextAsync(absolute, request.Content ?? string.Empty, ct);
        return Results.Created();
    }
}