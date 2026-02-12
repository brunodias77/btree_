using System.Security.Claims;
using BuildingBlocks.Security.Authentication;
using Microsoft.IdentityModel.Tokens;
using Users.Application.Services;

namespace Users.Infrastructure.Services;

/// <summary>
/// Implementação do serviço de tokens usando BuildingBlocks.Security.
/// </summary>
public class TokenService : ITokenService
{
    private readonly ITokenGenerator _tokenGenerator;

    public TokenService(ITokenGenerator tokenGenerator)
    {
        _tokenGenerator = tokenGenerator;
    }

    public string GenerateAccessToken(IEnumerable<Claim> claims)
    {
        var claimsList = claims.ToList();
        
        // Extrai claims obrigatórias
        var userId = Guid.Parse(claimsList.First(c => c.Type == ClaimTypes.NameIdentifier).Value);
        var email = claimsList.First(c => c.Type == ClaimTypes.Email).Value;
        
        // Extrai roles
        var roles = claimsList
            .Where(c => c.Type == ClaimTypes.Role)
            .Select(c => c.Value);

        // Extrai claims adicionais (excluindo as padrão já tratadas)
        var standardClaims = new[] { ClaimTypes.NameIdentifier, ClaimTypes.Email, ClaimTypes.Role };
        var additionalClaims = claimsList
            .Where(c => !standardClaims.Contains(c.Type))
            .ToDictionary(c => c.Type, c => c.Value);

        var result = _tokenGenerator.GenerateAccessToken(userId, email, roles, additionalClaims);
        return result.Token;
    }

    public (string Token, string TokenHash) GenerateRefreshToken()
    {
        var result = _tokenGenerator.GenerateRefreshToken();
        return (result.Token, result.TokenHash);
    }

    public ClaimsPrincipal GetPrincipalFromExpiredToken(string token)
    {
        var handler = new System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler();
        var userId = _tokenGenerator.GetUserIdFromExpiredToken(token);
        if (userId == null)
            throw new SecurityTokenException("Token inválido");

        var identity = new ClaimsIdentity(new[] 
        {
            new Claim(ClaimTypes.NameIdentifier, userId.ToString()!)
        });
        
        return new ClaimsPrincipal(identity);
    }
}
