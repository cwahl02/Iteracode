namespace Iteracode.Api.Models;

public class ProblemLanguage
{
	public  int  Id { get; set; }  
	public  int  ProblemId { get; set; }  
	public  Problem  Problem { get; set; } = null!;
  
	public  string  Language { get; set; } = null!; // e.g., "python", "cpp", "java"
	public  int  RunnerTemplateId { get; set; }  
	public  RunnerTemplate  RunnerTemplate { get; set; } = null!;  
  
	public  string  StarterCode { get; set; } = null!;  
	public  string  SolutionEntry { get; set; } = null!; // Function or class entry point  
	public  bool  Enabled { get; set; }  
	public  int  TimeLimitMs { get; set; } =  2000;  
	public  int  WallTimeLimitMs { get; set; } =  3000;  
	public  int  MemoryLimitKb { get; set; } =  524288; // default 512MB
}