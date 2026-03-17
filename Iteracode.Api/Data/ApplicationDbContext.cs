using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Iteracode.Api.Models;

namespace Iteracode.Api.Data;

public class ApplicationDbContext : IdentityDbContext<User>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options) { }

    public DbSet<BlacklistedToken> BlacklistedTokens => Set<BlacklistedToken>();
    public DbSet<LanguageJudge0Id> LanguageJudge0Ids => Set<LanguageJudge0Id>();
    public DbSet<Problem> Problems => Set<Problem>();
    public DbSet<ProblemLanguage> ProblemLanguages => Set<ProblemLanguage>();
    public DbSet<ProblemTag> ProblemTags => Set<ProblemTag>();
    public DbSet<ProblemTestcase> ProblemTestCases => Set<ProblemTestcase>();
    public DbSet<RunnerTemplate> RunnerTemplates => Set<RunnerTemplate>();
    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();
    public DbSet<Tag> Tags => Set<Tag>();
    public DbSet<Submission> Submissions => Set<Submission>();

    // Add to OnModelCreating:
    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<Problem>(e =>
        {
            e.HasKey(p => p.Id);
            e.HasIndex(p => p.Slug).IsUnique();
            e.Property(p => p.Slug).IsRequired().HasMaxLength(200);
        });

        builder.Entity<Submission>(e =>
        {
            e.HasKey(s => s.Id);
            e.HasOne(s => s.User)
            .WithMany()
            .HasForeignKey(s => s.UserId)
            .OnDelete(DeleteBehavior.Cascade);
            e.HasOne(s => s.Problem)
            .WithMany(p => p.Submissions)
            .HasForeignKey(s => s.ProblemSlug)
            .HasPrincipalKey(p => p.Slug)
            .OnDelete(DeleteBehavior.Cascade);
            e.Property(s => s.Language).IsRequired().HasMaxLength(50);
        });
    }
}