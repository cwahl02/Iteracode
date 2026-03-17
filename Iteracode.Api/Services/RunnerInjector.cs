using Iteracode.Api.InjectionMarkers;

namespace Iteracode.Api.Services;

public interface IRunnerInjector
{
    /// <summary>
    /// Replaces all tokens in the template and determines whether TestCase.Input
    /// should be passed as stdin or was already embedded in the source.
    /// </summary>
    (string assembledSource, string? stdin) Inject(
        string template,
        string inputMode,
        string userCode,
        string? entryPoint,
        string? className,
        string testInput);
}

public class RunnerInjector : IRunnerInjector, ISingletonService
{
    public (string assembledSource, string? stdin) Inject(
        string template,
        string inputMode,
        string userCode,
        string? entryPoint,
        string? className,
        string testInput)
    {
        var isTokenMode = inputMode.Equals("token", StringComparison.OrdinalIgnoreCase);

        var source = template
            .Replace("{USER_CODE}",    userCode)
            .Replace("{ENTRY}",        entryPoint ?? string.Empty)
            .Replace("{CLASS_NAME}",   className  ?? string.Empty)
            .Replace("{TESTS}",        isTokenMode ? testInput : string.Empty)
            .Replace("{METHOD_CALLS}", isTokenMode ? testInput : string.Empty);

        var stdin = isTokenMode ? null : testInput;

        return (source, stdin);
    }
}