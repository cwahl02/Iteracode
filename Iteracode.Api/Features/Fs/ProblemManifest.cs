using System.Text.Json.Serialization;

namespace Iteracode.Api.Features.Fs;

public record ProblemManifest(
    [property: JsonPropertyName("slug")]       string Slug,
    [property: JsonPropertyName("title")]      string Title,
    [property: JsonPropertyName("published")]  bool Published,
    [property: JsonPropertyName("description")]string Description,
    [property: JsonPropertyName("tags")]       List<string> Tags,
    [property: JsonPropertyName("languages")]  Dictionary<string, LanguageConfig> Languages
);

public record LanguageConfig(
    [property: JsonPropertyName("fileDir")]  string FileDir,
    [property: JsonPropertyName("files")]    List<string> Files,
    [property: JsonPropertyName("runner")]   string Runner
);