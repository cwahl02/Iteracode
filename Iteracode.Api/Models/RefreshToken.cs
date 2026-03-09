namespace Iteracode.Api.Models;

public class RefreshToken
{
    public int Id { get; set; }
    public string UserId { get; set; } = string.Empty;
    public User User { get; set; } = null!;
    public string TokenHash { get; set; } = string.Empty;
    public string? DeviceInfo { get; set; }
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
    public DateTimeOffset Expiry { get; set; }
    public DateTimeOffset? RevokedAt { get; set; }
}