using Iteracode.Api.Models;

namespace Iteracode.Api.Abstractions;

public interface IJudge0Client
{
    Task<Judge0Result> RunAsync(
        string sourceCode,
        int languageId,
        string? stdin = null,
        CancellationToken cancellationToken = default);
}