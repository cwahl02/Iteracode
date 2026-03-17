using System.Text.Json.Serialization;

namespace Iteracode.Api.Models;

public record Judge0Result(
    [property: JsonPropertyName("stdout")]  string? Stdout,
    [property: JsonPropertyName("stderr")]  string? Stderr,
    [property: JsonPropertyName("compile_output")]  string? CompileOutput,
    [property: JsonPropertyName("exit_code")]  int ExitCode,
    [property: JsonPropertyName("message")]  string? Message,
    [property: JsonPropertyName("status")]  Judge0Status Status
);

public record Judge0Status(
    [property: JsonPropertyName("id")] int Id,
    [property: JsonPropertyName("description")] string Description
);

public record Judge0SubmissionRequest(
    [property: JsonPropertyName("source_code")] string SourceCode,
    [property: JsonPropertyName("language_id")] int LanguageId,
    [property: JsonPropertyName("stdin")] string? Stdin,
    [property: JsonPropertyName("cpu_time_limit")] int CpuTimeLimitSeconds = 5,
    [property: JsonPropertyName("memory_limit")] int MemoryLimitKb = 65536
);


public record Judge0SubmissionToken(
    [property: JsonPropertyName("token")] string Token);