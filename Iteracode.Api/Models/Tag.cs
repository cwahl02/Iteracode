namespace Iteracode.Api.Models;

public class Tag
{
    public int Id { get; set; }
    public string Name { get; set; } = null!; // e.g., "arrays", "dynamic programming", "greedy"
    public DateTimeOffset CreatedAt { get; set; }
    public ICollection<ProblemTag> ProblemTags { get; set; } = null!;
}