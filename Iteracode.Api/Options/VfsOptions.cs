namespace Iteracode.Api.Options;

public record VfsOptions
{
    public string RootPath { get; init; } = "vfs";
    public int RecycleBinExpiryDays { get; init; } = 30;
}