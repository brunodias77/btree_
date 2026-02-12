namespace BuildingBlocks.Security.Authentication;

/// <summary>
/// Configuração para autenticação JWT.
/// </summary>
public sealed class JwtConfiguration
{
    /// <summary>
    /// Nome da seção de configuração.
    /// </summary>
    public const string SectionName = "Jwt";

    /// <summary>
    /// Chave secreta para assinatura dos tokens.
    /// Deve ter pelo menos 32 caracteres (256 bits).
    /// </summary>
    public string SecretKey { get; set; } = string.Empty;

    /// <summary>
    /// Emissor do token.
    /// Geralmente a URL da API.
    /// </summary>
    public string Issuer { get; set; } = string.Empty;

    /// <summary>
    /// Audiência do token.
    /// Geralmente a URL do cliente ou identificador da aplicação.
    /// </summary>
    public string Audience { get; set; } = string.Empty;

    /// <summary>
    /// Tempo de expiração do access token.
    /// Padrão: 15 minutos.
    /// </summary>
    public TimeSpan AccessTokenExpiration { get; set; } = TimeSpan.FromMinutes(15);

    /// <summary>
    /// Tempo de expiração do refresh token.
    /// Mapeado para coluna 'expires_at' em users.sessions.
    /// Padrão: 7 dias.
    /// </summary>
    public TimeSpan RefreshTokenExpiration { get; set; } = TimeSpan.FromDays(7);

    /// <summary>
    /// Permite refresh tokens serem usados múltiplas vezes.
    /// Quando false, o refresh token é rotacionado a cada uso (mais seguro).
    /// Padrão: false.
    /// </summary>
    public bool AllowMultipleRefreshes { get; set; } = false;

    /// <summary>
    /// Valida a configuração.
    /// </summary>
    public void Validate()
    {
        if (string.IsNullOrEmpty(SecretKey) || SecretKey.Length < 32)
            throw new InvalidOperationException("JWT SecretKey deve ter pelo menos 32 caracteres.");

        if (string.IsNullOrEmpty(Issuer))
            throw new InvalidOperationException("JWT Issuer é obrigatório.");

        if (string.IsNullOrEmpty(Audience))
            throw new InvalidOperationException("JWT Audience é obrigatório.");

        if (AccessTokenExpiration <= TimeSpan.Zero)
            throw new InvalidOperationException("AccessTokenExpiration deve ser maior que zero.");

        if (RefreshTokenExpiration <= AccessTokenExpiration)
            throw new InvalidOperationException("RefreshTokenExpiration deve ser maior que AccessTokenExpiration.");
    }
}
