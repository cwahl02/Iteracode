namespace Iteracode.Api.Abstractions;

public interface IBlacklistService
{
    Task RevokeAsync(string jti, DateTimeOffset tokenExpiry);
    Task<bool> IsRevokedAsync(string jti);
}