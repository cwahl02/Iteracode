using Iteracode.Api.Abstractions;
using Iteracode.Api.Data;
using Iteracode.Api.InjectionMarkers;
using Iteracode.Api.Models;
using Microsoft.EntityFrameworkCore;

public class BlacklistService : IBlacklistService, IScopedService
{
    private readonly ApplicationDbContext _context;

    public BlacklistService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task RevokeAsync(string jti, DateTimeOffset tokenExpiry)
    {
        var blacklistedToken = new BlacklistedToken
        {
            Jti = $"blacklist:{jti}",
            ExpiresAt = tokenExpiry
        };

        _context.BlacklistedTokens.Add(blacklistedToken);
        await _context.SaveChangesAsync();
    }

    public async Task<bool> IsRevokedAsync(string jti)
    {
        var key = $"blacklist:{jti}";
        var now = DateTimeOffset.UtcNow;

        // Purge expired rows opportunistically
        await _context.BlacklistedTokens
            .Where(t => t.ExpiresAt <= now)
            .ExecuteDeleteAsync();

        return await _context.BlacklistedTokens.AnyAsync(t => t.Jti == key);
    }
}