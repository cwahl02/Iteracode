namespace Iteracode.Api.Models;

public class ProblemTag
{
    public int Id { get; set; }
    public int ProblemId { get; set; }
    public Problem Problem { get; set; } = null!;

    public int TagId { get; set; }
    public Tag Tag { get; set; } = null!;
}