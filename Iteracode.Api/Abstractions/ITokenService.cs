using Iteracode.Api.Models;

namespace Iteracode.Api.Abstractions;

public interface ITokenService
{
    string GenerateAccessToken(User user);
    (string raw, string hashed) GenerateRefreshToken();
}