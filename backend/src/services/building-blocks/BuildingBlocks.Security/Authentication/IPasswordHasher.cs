namespace BuildingBlocks.Security.Authentication;

/// <summary>
/// Interface para hash de senhas.
/// Abstração sobre algoritmos de hash para permitir evolução sem quebrar compatibilidade.
/// </summary>
public interface IPasswordHasher
{
    /// <summary>
    /// Gera hash de uma senha.
    /// </summary>
    /// <param name="password">Senha em texto plano.</param>
    /// <returns>Hash da senha.</returns>
    string HashPassword(string password);

    /// <summary>
    /// Verifica se uma senha corresponde ao hash armazenado.
    /// </summary>
    /// <param name="password">Senha em texto plano para verificar.</param>
    /// <param name="hashedPassword">Hash armazenado.</param>
    /// <returns>True se a senha corresponde ao hash.</returns>
    bool VerifyPassword(string password, string hashedPassword);

    /// <summary>
    /// Verifica se o hash precisa ser atualizado (algoritmo mais novo disponível).
    /// </summary>
    /// <param name="hashedPassword">Hash armazenado.</param>
    /// <returns>True se o hash deve ser recalculado.</returns>
    bool NeedsRehash(string hashedPassword);
}
