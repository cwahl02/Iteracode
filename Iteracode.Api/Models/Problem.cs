namespace Iteracode.Api.Models;

public class Problem
{
    public int Id { get; set; }
    public string Slug { get; set; } = null!;
    public bool Published { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }
    public DateTimeOffset? DeletedAt { get; set; }

    // Nav
    public ICollection<Submission> Submissions { get; set; } = [];
}