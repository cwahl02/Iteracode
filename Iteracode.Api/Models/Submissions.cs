namespace Iteracode.Api.Models;

public class Submission
{
    public int Id { get; set; }
    
    public int UserId { get; set; } // FK → Users table
    public User User { get; set; } = null!;
    
    public int ProblemId { get; set; }
    public Problem Problem { get; set; } = null!;

    public int ProblemLanguageId { get; set; }
    public ProblemLanguage ProblemLanguage { get; set; } = null!;

    public int RunnerTemplateId { get; set; }
    public RunnerTemplate RunnerTemplate { get; set; } = null!;

    public string CodeSubmitted { get; set; } = null!;
    public string EntryName { get; set; } = null!;

    public string Status { get; set; } = null!; // PASS/FAIL/ERROR
    public int Score { get; set; }
    public bool Completed { get; set; }

    public string Stdout { get; set; } = null!;
    public string Stderr { get; set; } = null!;

    public int CpuTimeMs { get; set; }
    public int WallTimeMs { get; set; }
    public int MemoryKb { get; set; }

    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
}