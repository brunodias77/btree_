using System.Security.Cryptography;

namespace BuildingBlocks.Security.Authentication;

/// <summary>
/// Implementação de hash de senhas usando PBKDF2.
/// Algoritmo recomendado para senhas com iterações configuráveis.
/// </summary>
public sealed class PasswordHasher : IPasswordHasher
{
    // Configurações padrão (podem ser aumentadas ao longo do tempo)
    private const int SaltSize = 16; // 128 bits
    private const int HashSize = 32; // 256 bits
    private const int Iterations = 100000; // OWASP recomenda 100k+ para PBKDF2-SHA256
    private const char Delimiter = ':';

    /// <summary>
    /// Gera hash de uma senha usando PBKDF2-SHA256.
    /// Formato: iterations:salt:hash (base64).
    /// </summary>
    public string HashPassword(string password)
    {
        if (string.IsNullOrEmpty(password))
            throw new ArgumentException("Senha não pode ser vazia.", nameof(password));

        // Gera salt aleatório
        var salt = RandomNumberGenerator.GetBytes(SaltSize);

        // Gera hash usando PBKDF2
        var hash = Rfc2898DeriveBytes.Pbkdf2(
            password,
            salt,
            Iterations,
            HashAlgorithmName.SHA256,
            HashSize);

        // Retorna no formato: iterations:salt:hash
        return $"{Iterations}{Delimiter}{Convert.ToBase64String(salt)}{Delimiter}{Convert.ToBase64String(hash)}";
    }

    /// <summary>
    /// Verifica se uma senha corresponde ao hash armazenado.
    /// </summary>
    public bool VerifyPassword(string password, string hashedPassword)
    {
        if (string.IsNullOrEmpty(password) || string.IsNullOrEmpty(hashedPassword))
            return false;

        try
        {
            var parts = hashedPassword.Split(Delimiter);
            if (parts.Length != 3)
                return false;

            var iterations = int.Parse(parts[0]);
            var salt = Convert.FromBase64String(parts[1]);
            var storedHash = Convert.FromBase64String(parts[2]);

            // Gera hash da senha fornecida com o mesmo salt e iterações
            var computedHash = Rfc2898DeriveBytes.Pbkdf2(
                password,
                salt,
                iterations,
                HashAlgorithmName.SHA256,
                storedHash.Length);

            // Comparação em tempo constante para evitar timing attacks
            return CryptographicOperations.FixedTimeEquals(storedHash, computedHash);
        }
        catch
        {
            return false;
        }
    }

    /// <summary>
    /// Verifica se o hash precisa ser atualizado.
    /// Útil quando aumentamos o número de iterações.
    /// </summary>
    public bool NeedsRehash(string hashedPassword)
    {
        if (string.IsNullOrEmpty(hashedPassword))
            return true;

        try
        {
            var parts = hashedPassword.Split(Delimiter);
            if (parts.Length != 3)
                return true;

            var storedIterations = int.Parse(parts[0]);

            // Se o número de iterações armazenado é menor que o atual, precisa refazer
            return storedIterations < Iterations;
        }
        catch
        {
            return true;
        }
    }
}
