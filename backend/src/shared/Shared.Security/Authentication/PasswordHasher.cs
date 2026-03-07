using Shared.Security.Abstractions;

namespace Shared.Security.Authentication;

public sealed class PasswordHasher : IPasswordHasher
{
    // Work factor padrão (2^12 = 4096 iterações). Aumentar se necessário.
    private const int WorkFactor = 12;

    public string HashPassword(string password)
    {
        return BCrypt.Net.BCrypt.HashPassword(password, WorkFactor);
    }

    public bool VerifyPassword(string password, string hashedPassword)
    {
        return BCrypt.Net.BCrypt.Verify(password, hashedPassword);
    }

    public bool NeedsRehash(string hashedPassword)
    {
        // Verifica se o hash foi gerado com um work factor menor que o atual
        var hashInfo = BCrypt.Net.BCrypt.PasswordNeedsRehash(hashedPassword, WorkFactor);
        return hashInfo;
    }
}