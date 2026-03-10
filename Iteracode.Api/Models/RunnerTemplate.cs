namespace Iteracode.Api.Models;

public class RunnerTemplate
{
    public int Id { get; set; }
    public string Name { get; set; } = null!; // e.g., "Python Script", "C Function"
    public string TemplateCode { get; set; } = null!;
    public string Description { get; set; } = null!;
    public DateTimeOffset CreatedAt { get; set; }
}