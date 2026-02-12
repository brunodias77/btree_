using System.Security.Claims;

namespace BuildingBlocks.Security.Authentication;

/// <summary>
/// Interface para geração de tokens de autenticação.
/// Suporta JWT access tokens e refresh tokens.
/// </summary>
public interface ITokenGenerator
{
    /// <summary>
    /// Gera um access token JWT.
    /// </summary>
    /// <param name="userId">ID do usuário.</param>
    /// <param name="email">Email do usuário.</param>
    /// <param name="roles">Roles do usuário.</param>
    /// <param name="additionalClaims">Claims adicionais (opcional).</param>
    /// <returns>Token JWT e metadados.</returns>
    TokenResult GenerateAccessToken(
        Guid userId,
        string email,
        IEnumerable<string> roles,
        IDictionary<string, string>? additionalClaims = null);

    /// <summary>
    /// Gera um refresh token.
    /// </summary>
    /// <returns>Refresh token e hash para armazenamento.</returns>
    RefreshTokenResult GenerateRefreshToken();

    /// <summary>
    /// Valida um token e retorna as claims.
    /// </summary>
    /// <param name="token">Token JWT.</param>
    /// <returns>Claims do token se válido, null caso contrário.</returns>
    ClaimsPrincipal? ValidateToken(string token);

    /// <summary>
    /// Obtém o ID do usuário a partir de um token expirado (para refresh).
    /// </summary>
    /// <param name="expiredToken">Token expirado.</param>
    /// <returns>ID do usuário ou null se inválido.</returns>
    Guid? GetUserIdFromExpiredToken(string expiredToken);
}

/// <summary>
/// Resultado da geração de access token.
/// </summary>
public sealed record TokenResult
{
    /// <summary>
    /// Token JWT.
    /// </summary>
    public required string Token { get; init; }

    /// <summary>
    /// Data/hora de expiração (UTC).
    /// </summary>
    public required DateTime ExpiresAt { get; init; }

    /// <summary>
    /// Tipo do token (sempre "Bearer").
    /// </summary>
    public string TokenType => "Bearer";

    /// <summary>
    /// Tempo de vida em segundos.
    /// </summary>
    public int ExpiresIn => (int)(ExpiresAt - DateTime.UtcNow).TotalSeconds;
}

/// <summary>
/// Resultado da geração de refresh token.
/// </summary>
public sealed record RefreshTokenResult
{
    /// <summary>
    /// Token para enviar ao cliente.
    /// </summary>
    public required string Token { get; init; }

    /// <summary>
    /// Hash do token para armazenar no banco.
    /// Mapeado para coluna 'refresh_token_hash' em users.sessions.
    /// </summary>
    public required string TokenHash { get; init; }

    /// <summary>
    /// Data/hora de expiração (UTC).
    /// Mapeado para coluna 'expires_at' em users.sessions.
    /// </summary>
    public required DateTime ExpiresAt { get; init; }
}
