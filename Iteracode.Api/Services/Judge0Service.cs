using System.Net.Http.Json;
using System.Text.Json.Serialization;
using Iteracode.Api.InjectionMarkers;
using System.Text.Json;
using System.Text;

namespace Iteracode.Api.Services;

public interface IJudge0Service
{
    Task<Judge0Result> RunAsync(string code, int languageId, string? stdin = null);
}

public class Judge0Service : IJudge0Service
{
    private readonly HttpClient _httpClient;

    public Judge0Service(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<Judge0Result> RunAsync(string code, int languageId, string? stdin = null)
    {
        var request = new Judge0Request(code, languageId, stdin);

        var json = JsonSerializer.Serialize(request);
        Console.WriteLine($"[Judge0] Sending: {json}"); // check this in your console

        var content = new StringContent(json, Encoding.UTF8, "application/json");
        var response = await _httpClient.PostAsync(
            "/submissions?base64_encoded=false&wait=true",
            content);

        var responseBody = await response.Content.ReadAsStringAsync();
        Console.WriteLine($"[Judge0] Response ({(int)response.StatusCode}): {responseBody}");

        response.EnsureSuccessStatusCode();

        return JsonSerializer.Deserialize<Judge0Result>(responseBody)!;
    }
}

public record Judge0Result(
    [property: JsonPropertyName("stdout")] string? Stdout,
    [property: JsonPropertyName("stderr")] string? Stderr,
    [property: JsonPropertyName("compile_output")] string? CompileOutput,
    [property: JsonPropertyName("status")] Judge0Status Status);

public record Judge0Status(
    [property: JsonPropertyName("id")] int Id,
    [property: JsonPropertyName("description")] string Description);

file record Judge0Request(
    [property: JsonPropertyName("source_code")] string SourceCode,
    [property: JsonPropertyName("language_id")] int LanguageId,
    [property: JsonPropertyName("stdin")] string? Stdin);