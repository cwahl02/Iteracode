namespace Iteracode.Api.Models;

public class LanguageJudge0Id
{
	public int Id { get; set;}
	public string Language { get; set; } = null!; // e.g., "python", "c", "cpp"
	public int Judge0Id { get; set; }
	public bool Enabled { get; set; }
	public DateTimeOffset CreatedAt { get; set; }
}