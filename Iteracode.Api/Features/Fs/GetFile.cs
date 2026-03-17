using Iteracode.Api.Abstractions;
using Iteracode.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace Iteracode.Api.Features.Fs;

public sealed class GetFile : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/fs/file", Handle)
            .WithTags("FS")
            .RequireAuthorization()
            .Produces<object>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound);
    }

    public static async Task<IResult> Handle(
        string path,
        [FromServices] IVfsService vfs,
        CancellationToken ct)
    {
        var absolute = vfs.Resolve(path);
        if (!File.Exists(absolute))
            return Results.NotFound();
        var content = await File.ReadAllTextAsync(absolute, ct);
        return Results.Ok(new { content });
    }
}