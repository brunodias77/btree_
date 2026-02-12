using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace BuildingBlocks.Security.Authentication;

/// <summary>
/// Implementação de geração de tokens JWT.
/// Mapeado para tabela users.sessions do schema.
/// </summary>
public sealed class JwtTokenGenerator : ITokenGenerator
{
    private readonly JwtConfiguration _config;
    private readonly SigningCredentials _signingCredentials;
    private readonly TokenValidationParameters _validationParameters;

    public JwtTokenGenerator(IOptions<JwtConfiguration> config)
    {
        _config = config.Value;

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config.SecretKey));
        _signingCredentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        _validationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = _config.Issuer,
            ValidAudience = _config.Audience,
            IssuerSigningKey = key,
            ClockSkew = TimeSpan.Zero // Sem tolerância de tempo
        };
    }

    /// <summary>
    /// Gera um access token JWT.
    /// </summary>
    public TokenResult GenerateAccessToken(
        Guid userId,
        string email,
        IEnumerable<string> roles,
        IDictionary<string, string>? additionalClaims = null)
    {
        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, userId.ToString()),
            new(JwtRegisteredClaimNames.Email, email),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new(JwtRegisteredClaimNames.Iat, DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString(), ClaimValueTypes.Integer64)
        };

        // Adiciona roles
        foreach (var role in roles)
        {
            claims.Add(new Claim(ClaimTypes.Role, role));
        }

        // Adiciona claims adicionais
        if (additionalClaims is not null)
        {
            foreach (var (key, value) in additionalClaims)
            {
                claims.Add(new Claim(key, value));
            }
        }

        var expiresAt = DateTime.UtcNow.Add(_config.AccessTokenExpiration);

        var token = new JwtSecurityToken(
            issuer: _config.Issuer,
            audience: _config.Audience,
            claims: claims,
            expires: expiresAt,
            signingCredentials: _signingCredentials);

        return new TokenResult
        {
            Token = new JwtSecurityTokenHandler().WriteToken(token),
            ExpiresAt = expiresAt
        };
    }

    /// <summary>
    /// Gera um refresh token.
    /// O token é um valor aleatório seguro, armazenamos o hash no banco.
    /// </summary>
    public RefreshTokenResult GenerateRefreshToken()
    {
        // Gera 64 bytes aleatórios para o token
        var tokenBytes = RandomNumberGenerator.GetBytes(64);
        var token = Convert.ToBase64String(tokenBytes);

        // Gera hash SHA256 para armazenar no banco
        var hashBytes = SHA256.HashData(Encoding.UTF8.GetBytes(token));
        var tokenHash = Convert.ToBase64String(hashBytes);

        return new RefreshTokenResult
        {
            Token = token,
            TokenHash = tokenHash,
            ExpiresAt = DateTime.UtcNow.Add(_config.RefreshTokenExpiration)
        };
    }

    /// <summary>
    /// Valida um token e retorna as claims.
    /// </summary>
    public ClaimsPrincipal? ValidateToken(string token)
    {
        if (string.IsNullOrEmpty(token))
            return null;

        try
        {
            var handler = new JwtSecurityTokenHandler();
            var principal = handler.ValidateToken(token, _validationParameters, out _);
            return principal;
        }
        catch
        {
            return null;
        }
    }

    /// <summary>
    /// Obtém o ID do usuário a partir de um token expirado.
    /// Usado durante o refresh para identificar o usuário.
    /// </summary>
    public Guid? GetUserIdFromExpiredToken(string expiredToken)
    {
        if (string.IsNullOrEmpty(expiredToken))
            return null;

        try
        {
            var validationParams = _validationParameters.Clone();
            validationParams.ValidateLifetime = false; // Ignora expiração

            var handler = new JwtSecurityTokenHandler();
            var principal = handler.ValidateToken(expiredToken, validationParams, out _);

            var userIdClaim = principal.FindFirst(JwtRegisteredClaimNames.Sub)
                ?? principal.FindFirst(ClaimTypes.NameIdentifier);

            if (userIdClaim is null || !Guid.TryParse(userIdClaim.Value, out var userId))
                return null;

            return userId;
        }
        catch
        {
            return null;
        }
    }

    /// <summary>
    /// Gera hash de um refresh token para comparação.
    /// </summary>
    public static string HashRefreshToken(string token)
    {
        var hashBytes = SHA256.HashData(Encoding.UTF8.GetBytes(token));
        return Convert.ToBase64String(hashBytes);
    }
}
