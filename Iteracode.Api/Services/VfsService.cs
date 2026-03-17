using Iteracode.Api.Abstractions;
using Iteracode.Api.InjectionMarkers;
using Iteracode.Api.Options;
using Microsoft.Extensions.Options;

namespace Iteracode.Api.Services;

public class VfsService : IVfsService, ISingletonService
{
    private readonly string _root;

    private static readonly HashSet<string> ProtectedPaths =
    [
        "root",
        "root/recycled",
        "root/problems",
        "root/problems/scripts"
    ];

    public VfsService(IOptions<VfsOptions> options)
    {
        _root = Path.GetFullPath(options.Value.RootPath);
    }

    public string Resolve(string vfsPath)
    {
        var full = Path.GetFullPath(Path.Combine(_root, vfsPath));
        if (!full.StartsWith(_root, StringComparison.OrdinalIgnoreCase))
            throw new InvalidOperationException("Path traversal detected.");
        return full;
    }

    public string ToVfsPath(string absolutePath) =>
        Path.GetRelativePath(_root, absolutePath).Replace("\\", "/");

    public bool IsProtected(string vfsPath) =>
        ProtectedPaths.Contains(vfsPath.TrimEnd('/'));

    public bool IsProblemJson(string vfsPath) =>
        vfsPath.EndsWith("/problem.json", StringComparison.OrdinalIgnoreCase);

    // e.g. root/problems/two-sum/problem.json → "two-sum"
    public string GetParentProblemSlug(string vfsPath)
    {
        var parts = vfsPath.Split('/');
        // root/problems/{slug}/problem.json
        if (parts.Length == 4 && parts[0] == "root" && parts[1] == "problems")
            return parts[2];
        return string.Empty;
    }

    // e.g. root/problems/two-sum → root/recycled/2026-03-17-two-sum
    public string BuildRecyclePath(string slug)
    {
        var prefix = DateTimeOffset.UtcNow.ToString("yyyy-MM-dd");
        return $"root/recycled/{prefix}-{slug}";
    }

    // e.g. root/recycled/2026-03-17-two-sum → "two-sum"
    public string? TryExtractRecycledSlug(string vfsPath)
    {
        var parts = vfsPath.Split('/');
        if (parts.Length != 3 || parts[0] != "root" || parts[1] != "recycled")
            return null;
        var name = parts[2];
        // strip yyyy-MM-dd- prefix
        var idx = name.IndexOf('-', 11); // at least yyyy-MM-dd = 10 chars + dash
        return idx > 0 ? name[(idx + 1)..] : null;
    }
}