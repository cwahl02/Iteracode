using Iteracode.Api.Data;
using Iteracode.Api.Options;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace Iteracode.Api.Services;

public class RecyclePurgeService : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly IVfsService _vfs;
    private readonly int _expiryDays;

    public RecyclePurgeService(
        IServiceScopeFactory scopeFactory,
        IVfsService vfs,
        IOptions<VfsOptions> options)
    {
        _scopeFactory = scopeFactory;
        _vfs          = vfs;
        _expiryDays   = options.Value.RecycleBinExpiryDays;
    }

    protected override async Task ExecuteAsync(CancellationToken ct)
    {
        while (!ct.IsCancellationRequested)
        {
            await PurgeExpiredAsync(ct);
            // run once per day
            await Task.Delay(TimeSpan.FromHours(24), ct);
        }
    }

    private async Task PurgeExpiredAsync(CancellationToken ct)
    {
        var recycledAbs = _vfs.Resolve("root/recycled");
        if (!Directory.Exists(recycledAbs)) return;

        var cutoff = DateTimeOffset.UtcNow.AddDays(-_expiryDays);

        foreach (var dir in Directory.EnumerateDirectories(recycledAbs))
        {
            var name = Path.GetFileName(dir);
            // parse yyyy-MM-dd prefix
            if (!DateTimeOffset.TryParse(name[..10], out var recycledDate)) continue;
            if (recycledDate > cutoff) continue;

            try
            {
                Directory.Delete(dir, recursive: true);

                // remove DB row
                var slug = _vfs.TryExtractRecycledSlug(
                    _vfs.ToVfsPath(dir));

                if (!string.IsNullOrEmpty(slug))
                {
                    using var scope = _scopeFactory.CreateScope();
                    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                    var problem = await db.Problems
                        .FirstOrDefaultAsync(p => p.Slug == slug, ct);
                    if (problem is not null)
                    {
                        db.Problems.Remove(problem);
                        await db.SaveChangesAsync(ct);
                    }
                }
            }
            catch { /* log and continue */ }
        }
    }
}