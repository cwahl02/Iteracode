using System.ComponentModel.DataAnnotations;

namespace Iteracode.Api.Models;

public class BlacklistedToken
{
    [Key]
    public string Jti { get; set; } = null!;
    public DateTimeOffset ExpiresAt { get; set; }
}