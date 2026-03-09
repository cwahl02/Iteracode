using System.Security.Cryptography;
using System.Text;
using Iteracode.Api.Abstractions;
using Iteracode.Api.InjectionMarkers;

public class Sha256HashingService : IHashingService, IScopedService
{
    public string Hash(byte[] data)
    {
        var hash = SHA256.HashData(data);
        return Convert.ToBase64String(hash);
    }

    public string Hash(string data)
        => Hash(Encoding.UTF8.GetBytes(data));

    public bool Verify(string hash, byte[] data)
        => Hash(data) == hash;

    public bool Verify(string hash, string data)
        => Hash(data) == hash;
}