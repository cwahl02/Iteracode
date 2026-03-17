namespace Iteracode.Api.Options;

public record Judge0Options
{
    public string BaseUrl { get; init; } = string.Empty;
    public string? AuthToken { get; init; }
    public string ApiKey { get; init; } = string.Empty;
}