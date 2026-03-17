namespace Iteracode.Api.Models;

public class Submission
{
    public int Id { get; set; }
    public string UserId { get; set; } = null!;
    public string ProblemSlug { get; set; } = null!;
    public string Language { get; set; } = null!;
    public bool Passed { get; set; }
    public DateTimeOffset SubmittedAt { get; set; }

    // Nav
    public User User { get; set; } = null!;
    public Problem Problem { get; set; } = null!;
}