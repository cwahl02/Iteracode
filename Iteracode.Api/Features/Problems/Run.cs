// using System.Security.Claims;
// using Iteracode.Api.Abstractions;
// using Iteracode.Api.Data;
// using Iteracode.Api.Models;
// using Iteracode.Api.Services;
// using Microsoft.AspNetCore.Mvc;
// using Microsoft.EntityFrameworkCore;

// namespace Iteracode.Api.Features.Problems;

// public sealed class Run : IEndpoint
// {
//     public sealed record ProblemRunRequest(
//         int    ProblemId,
//         string Language,
//         string UserCode);

//     public sealed record TestCaseResult(
//         int     TestCaseId,
//         bool    Passed,
//         string? ActualOutput,
//         string? ExpectedOutput,
//         string? ErrorOutput,       // stderr or compile_output
//         int     StatusId,
//         string  StatusDescription);

//     public sealed record RunResponse(
//         bool                  Accepted,
//         int                   TestsPassed,
//         int                   TestsTotal,
//         List<TestCaseResult>  Results,
//         int                   SubmissionId);

//     public void MapEndpoint(IEndpointRouteBuilder app)
//     {
//         app.MapPost("/api/problem/run", Handle)
//             .WithTags("Problem")
//             .WithName("ProblemRun")
//             .WithSummary("Runs user code against a problem's test cases.")
//             .Produces<RunResponse>(StatusCodes.Status200OK)
//             .Produces(StatusCodes.Status400BadRequest)
//             .Produces(StatusCodes.Status404NotFound)
//             .RequireAuthorization();
//     }

//     public static async Task<IResult> Handle(
//         HttpContext httpContext,
//         [FromServices] ApplicationDbContext db,
//         [FromServices] IJudge0Client        judge0,
//         [FromServices] ILanguageService     languageService,
//         [FromServices] IRunnerInjector      injector,
//         [FromBody]     ProblemRunRequest           request,
//         CancellationToken ct)
//     {
//         var userId = httpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
//         if (userId is null) return Results.Unauthorized();

//         // 1. Load problem + ordered test cases
//         var problem = await db.Problems
//             .Include(p => p.TestCases.OrderBy(tc => tc.OrderIndex))
//             .FirstOrDefaultAsync(p => p.Id == request.ProblemId, ct);

//         if (problem is null)
//             return Results.NotFound(new { Error = $"Problem {request.ProblemId} not found." });

//         // 2. Load the ProblemLanguage row (includes RunnerTemplate via eager load)
//         var problemLanguage = await db.ProblemLanguages
//             .Include(pl => pl.RunnerTemplate)
//             .FirstOrDefaultAsync(pl =>
//                 pl.ProblemId == request.ProblemId &&
//                 pl.Language  == request.Language.ToLowerInvariant(), ct);

//         if (problemLanguage is null)
//             return Results.BadRequest(new
//             {
//                 Error = $"Language '{request.Language}' is not supported for problem {request.ProblemId}."
//             });

//         // 3. Resolve Judge0 language ID
//         int judge0LangId;
//         try
//         {
//             judge0LangId = await languageService.GetJudge0IdAsync(request.Language, ct);
//         }
//         catch (KeyNotFoundException ex)
//         {
//             return Results.BadRequest(new { Error = ex.Message });
//         }

//         var template = problemLanguage.RunnerTemplate;

//         // 4. Run each test case through the injector → Judge0
//         var testResults = new List<TestCaseResult>();

//         foreach (var tc in problem.TestCases)
//         {
//             var (assembledSource, stdin) = injector.Inject(
//                 template:    template.Template,
//                 inputMode:   template.InputMode,
//                 userCode:    request.UserCode,
//                 entryPoint:  problemLanguage.EntryPoint,
//                 className:   problemLanguage.ClassName,
//                 testInput:   tc.Input);

//             var judge0Result = await judge0.RunAsync(assembledSource, judge0LangId, stdin, ct);

//             // Judge0 status 3 = "Accepted" (ran successfully, no runtime error)
//             var actualOutput = (judge0Result.Stdout ?? string.Empty).Trim();
//             var expected     = tc.ExpectedOutput.Trim();
//             var passed       = judge0Result.Status.Id == 3 && actualOutput == expected;

//             testResults.Add(new TestCaseResult(
//                 tc.Id,
//                 passed,
//                 // Don't leak hidden test case output back to the client
//                 tc.IsHidden ? null : actualOutput,
//                 tc.IsHidden ? null : tc.ExpectedOutput,
//                 judge0Result.Stderr ?? judge0Result.CompileOutput,
//                 judge0Result.Status.Id,
//                 judge0Result.Status.Description));
//         }

//         // 5. Summarise
//         var testsPassed = testResults.Count(r => r.Passed);
//         var accepted    = testsPassed == testResults.Count;

//         // 6. Persist submission
//         var submission = new Submission
//         {
//             UserId       = userId,
//             ProblemId    = request.ProblemId,
//             Language     = request.Language.ToLowerInvariant(),
//             UserCode     = request.UserCode,
//             Accepted     = accepted,
//             TestsPassed  = testsPassed,
//             TestsTotal   = testResults.Count,
//             ErrorMessage = accepted
//                 ? null
//                 : testResults.FirstOrDefault(r => !r.Passed)?.ErrorOutput,
//             SubmittedAt  = DateTimeOffset.UtcNow
//         };

//         db.Submissions.Add(submission);
//         await db.SaveChangesAsync(ct);

//         return Results.Ok(new RunResponse(
//             accepted,
//             testsPassed,
//             testResults.Count,
//             testResults,
//             submission.Id));
//     }
// }