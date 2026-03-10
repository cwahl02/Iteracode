using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Iteracode.Api.Models;

namespace Iteracode.Api.Data;

public class ApplicationDbContext : IdentityDbContext<User>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options) { }

    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();
    public DbSet<BlacklistedToken> BlacklistedTokens => Set<BlacklistedToken>();
    public DbSet<Problem> Problems => Set<Problem>();
    public DbSet<ProblemLanguage> ProblemLanguages => Set<ProblemLanguage>();
    public DbSet<Testcase> TestCases => Set<Testcase>();
    public DbSet<ProblemTestcase> ProblemTestCases => Set<ProblemTestcase>();
    public DbSet<LanguageJudge0Id> LanguageJudge0Ids => Set<LanguageJudge0Id>();
    public DbSet<RunnerTemplate> RunnerTemplates => Set<RunnerTemplate>();

    // protected override void OnModelCreating(ModelBuilder builder)
    // {
    //     base.OnModelCreating(builder);
        
    //     // ProblemTestCase has a composite key of ProblemId and TestCaseId
    //     builder.Entity<ProblemTestCase>()
    //         .HasKey(ptc => new { ptc.ProblemId, ptc.TestCaseId });

    //     // Configure the relationship between ProblemTestCase and Problem
    //     builder.Entity<ProblemTestCase>()
    //         .HasOne(ptc => ptc.Problem)
    //         .WithMany(p => p.ProblemTestCases)
    //         .HasForeignKey(ptc => ptc.ProblemId)
    //         .OnDelete(DeleteBehavior.Cascade); // Delete ProblemTestCase when Problem is deleted

    //     // Configure the relationship between ProblemTestCase and TestCase
    //     builder.Entity<ProblemTestCase>()
    //         .HasOne(ptc => ptc.TestCase)
    //         .WithMany(tc => tc.ProblemTestCases)
    //         .HasForeignKey(ptc => ptc.TestCaseId)
    //         .OnDelete(DeleteBehavior.Restrict); // Don't delete TestCase when ProblemTestCase is deleted

    //     // Configure the relationship between ProblemImpl and Problem
    //     builder.Entity<ProblemImpl>()
    //         .HasOne(pi => pi.Problem)
    //         .WithMany(p => p.Implementations)
    //         .HasForeignKey(pi => pi.ProblemId)
    //         .OnDelete(DeleteBehavior.Cascade); // Delete ProblemImpl when Problem is deleted

    //     // Ensure that each Problem can only have one implementation per language
    //     builder.Entity<ProblemImpl>()
    //         .HasIndex(pi => new { pi.ProblemId, pi.LanguageId})
    //         .IsUnique();

    //     builder.Entity<Submission>()
    //         .HasOne(s => s.User)
    //         .WithMany()
    //         .HasForeignKey(s => s.UserId)
    //         .OnDelete(DeleteBehavior.Cascade); // Delete Submission when User is deleted

    //     builder.Entity<Submission>()
    //         .HasOne(s => s.Problem)
    //         .WithMany(p => p.Submissions)
    //         .HasForeignKey(s => s.ProblemId)
    //         .OnDelete(DeleteBehavior.SetNull) // Set ProblemId to null when Problem is deleted
    //         .IsRequired(false);

    //     builder.Entity<Submission>()
    //         .Property(s => s.Result)
    //         .HasConversion<string>(); // Store enum as string in the database
    // }
}