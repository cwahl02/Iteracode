public interface IVfsService
{
    string Resolve(string vfsPath);
    string ToVfsPath(string absolutePath);
    bool IsProtected(string vfsPath);
    bool IsProblemJson(string vfsPath);
    string GetParentProblemSlug(string vfsPath);
    string BuildRecyclePath(string slug);
    string? TryExtractRecycledSlug(string vfsPath);
}