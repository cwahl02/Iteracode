namespace Iteracode.Api.Models;

public  class  Testcase  
{  
	public  int  Id { get; set; }  
	public  string  InputJson { get; set; } = null!;  
	public  string  ExpectedOutput { get; set; } = null!;  
	public  DateTimeOffset  CreatedAt { get; set; }  
}