using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Iteracode.Api.Abstractions;
using Iteracode.Api.InjectionMarkers;
using Iteracode.Api.Models;
using Iteracode.Api.Options;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace Iteracode.Api.Services;

public class TokenService : ITokenService, IScopedService
{
    private readonly JwtOptions _jwtOptions;
    private readonly IHashingService _hashingService;

    public TokenService(IOptions<JwtOptions> jwtOptions, IHashingService hashingService)
    {
        _jwtOptions = jwtOptions.Value;
        _hashingService = hashingService;
    }

    public string GenerateAccessToken(User user)
    {
        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id),
            new Claim(JwtRegisteredClaimNames.Email, user.Email!),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim(JwtRegisteredClaimNames.Exp, DateTimeOffset.UtcNow.AddMinutes(_jwtOptions.AccessTokenExpiryMinutes).ToUnixTimeSeconds().ToString())
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtOptions.Secret));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _jwtOptions.Issuer,
            audience: _jwtOptions.Audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(_jwtOptions.AccessTokenExpiryMinutes),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public (string raw, string hashed) GenerateRefreshToken()
    {
        var raw = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
        var hashed = _hashingService.Hash(raw);
        return (raw, hashed);
    }
}