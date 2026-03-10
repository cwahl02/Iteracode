namespace Iteracode.Api.Models;

public class ProblemTestcase
{
    public int Id { get; set; }
    public int ProblemId { get; set; }
    public Problem Problem { get; set; } = null!;

    public int TestcaseId { get; set; }
    public Testcase Testcase { get; set; } = null!;

    public int OrderIndex { get; set; } = 0;
    public bool Visible { get; set; } = true; // optional: hide certain testcases
}