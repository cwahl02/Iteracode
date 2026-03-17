using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Iteracode.Api.Abstractions;
using Iteracode.Api.Models;

namespace Iteracode.Api.Services;

// ── Shared response types ─────────────────────────────────────────────────


// ── Client ────────────────────────────────────────────────────────────────
public class Judge0Client : IJudge0Client
{
    private static readonly JsonSerializerOptions JsonOpts = new()
    {
        PropertyNameCaseInsensitive = true
    };

    // Judge0 status IDs that mean "still running"
    private static readonly HashSet<int> PendingStatusIds = new() { 1, 2 }; // 1=In Queue, 2=Processing

    private readonly HttpClient _http;

    public Judge0Client(HttpClient http) => _http = http;

    public async Task<Judge0Result> RunAsync(
        string sourceCode,
        int languageId,
        string? stdin = null,
        CancellationToken ct = default)
    {
        // 1. Submit
        var token = await SubmitAsync(sourceCode, languageId, stdin, ct);

        // 2. Poll until complete
        return await PollAsync(token, ct);
    }

    private async Task<string> SubmitAsync(
        string sourceCode,
        int languageId,
        string? stdin,
        CancellationToken ct)
    {
        var payload = new Judge0SubmissionRequest(sourceCode, languageId, stdin);
        var content = new StringContent(
            JsonSerializer.Serialize(payload, JsonOpts),
            Encoding.UTF8,
            "application/json");

        var response = await _http.PostAsync("/submissions", content, ct);
        var body     = await response.Content.ReadAsStringAsync(ct);

        response.EnsureSuccessStatusCode();

        var submission = JsonSerializer.Deserialize<Judge0SubmissionToken>(body, JsonOpts)
            ?? throw new InvalidOperationException("Judge0 did not return a submission token.");

        return submission.Token;
    }

    private async Task<Judge0Result> PollAsync(string token, CancellationToken ct)
    {
        // Poll with backoff: 500ms, 500ms, 1s, 1s, 2s ... up to 20 attempts (~30s total)
        var delays = new[] { 500, 500, 1000, 1000, 1000, 2000, 2000, 2000, 2000, 2000 };

        for (var attempt = 0; ; attempt++)
        {
            var response = await _http.GetAsync($"/submissions/{token}", ct);
            var body     = await response.Content.ReadAsStringAsync(ct);
            response.EnsureSuccessStatusCode();

            var result = JsonSerializer.Deserialize<Judge0Result>(body, JsonOpts)
                ?? throw new InvalidOperationException("Empty response from Judge0.");

            // If no longer pending, we're done
            if (!PendingStatusIds.Contains(result.Status.Id))
                return result;

            // Cap delay at last value in array
            var delay = delays[Math.Min(attempt, delays.Length - 1)];
            await Task.Delay(delay, ct);
        }
    }
}