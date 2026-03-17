using System.Text.Json;

public record Parameter
{
    public string Name { get; init; } = null!;
    public string Type { get; init; } = null!;
}

public class CLanguageGenerator
{
    public Dictionary<string, string> TypeMap { get; } = new()
    {
        ["int"] = "int",
        ["string"] = "char*",
        ["int[]"] = "int*",
        ["string[]"] = "char**"
    };

    public string StructDefinition(IEnumerable<Parameter> parameters)
    {
        var fields = string.Join("\n    ", parameters.Select(p =>
            $"{TypeMap[p.Type]} {p.Name};"));

        return
            $"typedef struct {{\n" +
            $"    {fields}\n" +
            $"}} Args;";
    }

    public string FunctionCall(string functionName, IEnumerable<Parameter> parameters)
    {
        var args = string.Join(", ", parameters.Select(p => $"args.{p.Name}"));
        return $"{functionName}({args})";
    }

    public string ArrayLiteral(IEnumerable<string> values)
    {
        var elements = string.Join(", ", values);
        return $"{{ {elements} }}";
    }

    public string ValueLiteral(JsonElement value, string type)
        => type switch
        {
            "int"      => value.GetInt32().ToString(),
            "bool"     => value.GetBoolean() ? "1" : "0",
            "string"   => $"\"{value.GetString()}\"",
            "int[]"    => $"(int[]){ArrayLiteral(value.EnumerateArray().Select(e => e.GetInt32().ToString()))}",
            "string[]" => $"(char**){ArrayLiteral(value.EnumerateArray().Select(e => $"\"{e.GetString()}\""))}",
            _          => throw new NotSupportedException($"Unsupported type: {type}")
        };

    public string ArgStructLiteral(JsonElement inputs, IEnumerable<Parameter> parameters)
    {
        var fields = string.Join(", ", parameters.Select(p =>
            $".{p.Name} = {ValueLiteral(inputs.GetProperty(p.Name), p.Type)}"));

        return $"{{ {fields} }}";
    }

    public string TestCaseLiteral(JsonElement inputs, JsonElement expected, IEnumerable<Parameter> parameters, string returnType)
    {
        var args     = ArgStructLiteral(inputs, parameters);
        var exp      = ValueLiteral(expected, returnType);
        return $"{{ .args = {args}, .expected = {exp} }}";
    }
}