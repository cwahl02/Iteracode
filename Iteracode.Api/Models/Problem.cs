namespace Iteracode.Api.Models;

public class Problem
{
	public int Id { get; set; }
	public string Title { get; set; } = null!;
    public string Description { get; set; } = null!;
	public DateTimeOffset CreatedAt { get; set; }
	public ICollection<ProblemLanguage> ProblemLanguages { get; set; } = null!;
	public ICollection<ProblemTag> ProblemTags { get; set; } = null!;
	public ICollection<ProblemTestcase> ProblemTestcases { get; set; } = null!;
}