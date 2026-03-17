using Iteracode.Api.Abstractions;
using Iteracode.Api.Services;
using Microsoft.Extensions.Options;
using Iteracode.Api.Options;
using Microsoft.AspNetCore.Mvc;

namespace Iteracode.Api.Features.Fs;

public sealed class GetTree : IEndpoint
{
    public record VfsNodeDto(
        string Type,
        string Name,
        string Path,
        bool? Expanded,
        List<VfsNodeDto>? Children
    );

    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/fs/tree", Handle)
            .WithTags("FS")
            .RequireAuthorization()
            .Produces<object>(StatusCodes.Status200OK);
    }

    public static IResult Handle([FromServices] IVfsService vfs)
    {
        var rootAbsolute = vfs.Resolve("root");
        if (!Directory.Exists(rootAbsolute))
            Directory.CreateDirectory(rootAbsolute);

        var node = BuildNode(new DirectoryInfo(rootAbsolute), vfs);
        return Results.Ok(new { node });
    }

    private static VfsNodeDto BuildNode(FileSystemInfo info, IVfsService vfs)
    {
        var vfsPath = vfs.ToVfsPath(info.FullName);

        if (info is FileInfo fi)
            return new VfsNodeDto("file", fi.Name, vfsPath, null, null);

        var dir = (DirectoryInfo)info;
        var children = dir.EnumerateFileSystemInfos()
            .OrderBy(f => f is DirectoryInfo ? 0 : 1)
            .ThenBy(f => f.Name)
            .Select(f => BuildNode(f, vfs))
            .ToList();

        return new VfsNodeDto("folder", dir.Name, vfsPath, false, children);
    }
}