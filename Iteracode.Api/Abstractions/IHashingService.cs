namespace Iteracode.Api.Abstractions;

public interface IHashingService
{
    string Hash(byte[] data);
    string Hash(string data);
    bool Verify(string hash, byte[] data);
    bool Verify(string hash, string data);
}