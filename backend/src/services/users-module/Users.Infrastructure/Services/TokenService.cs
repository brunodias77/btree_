
using System.Security.Claims;
using BuildingBlocks.Security.Authentication;
using Microsoft.IdentityModel.Tokens;
using Users.Application.Abstractions;
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
        // Nota: O ITokenGenerator não expõe 'GetPrincipalFromExpiredToken' diretamente que retorne ClaimsPrincipal,
        // mas expõe ValidateToken. Contudo, ValidateToken falha se expirado.
        // O JwtTokenGenerator tem GetUserIdFromExpiredToken que ignora expiração.
        // Se precisarmos do Principal completo de um token expirado, teríamos que usar JwtSecurityTokenHandler manualmente
        // ou assumir que a validação aqui é apenas para extrair dados sem validar tempo.
        // O ITokenGenerator.ValidateToken valida tempo (ClockSkew Zero).
        
        // WORKAROUND: Para o fluxo de refresh, geralmente só precisamos do ID do usuário.
        // Mas a interface pede ClaimsPrincipal. 
        // Vamos tentar usar ValidateToken e capturar exceção se expirado? Não, queremos as claims mesmo expirado.
        
        // Como o Users.Application define a interface, e o BuildingBlocks é a implementação...
        // O BuildingBlocks deveria suportar isso. 
        
        // Vamos usar JwtSecurityTokenHandler manualmente aqui para extrair sem validar tempo, 
        // replicando a lógica segura do JwtTokenGenerator mas expondo o Principal.
        
        var handler = new System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler();
        // Não temos acesso aos parâmetros de validação (chave secreta) aqui facilmente sem injetar JwtConfiguration.
        // Mas o ITokenGenerator esconde isso.
        
        // Pelo contrato da interface ITokenService.GetPrincipalFromExpiredToken:
        // Assume-se que a implementação saiba como validar ignorando expiração.
        // O ITokenGenerator atual não oferece isso publicamente para retornar Principal.
        // Ele oferece GetUserIdFromExpiredToken.
        
        // Se mudarmos a interface para GetUserIdFromExpiredToken seria mais seguro e alinhado.
        // Mas vou manter a interface e chamar GetUserIdFromExpiredToken, e reconstruir um Principal básico?
        // Não, isso perde claims.
        
        // Vou verificar se ITokenGenerator pode ser extendido ou se uso reflection? Não.
        // Vou injetar IOptions<JwtConfiguration> aqui também para poder validar manualmente.
        
        // Devido à limitação, vou lançar NotImplementedException ou tentar o ValidateToken, assumindo que ainda não expirou
        // ou mudar a abordagem.
        
        // MELHOR ABORDAGEM: O JwtTokenGenerator em BuildingBlocks deveria ter esse método.
        // Mas não posso mudar BuildingBlocks agora facilmente (outra conversa).
        
        // Vou injetar TokenValidationParameters? Não.
        
        // Vou usar o GetUserIdFromExpiredToken do ITokenGenerator para pegar o ID, 
        // e se precisar de outras claims, buscamos do banco (o que é mais seguro para Refresh Token flow).
        // No Refresh Token flow, validamos o Refresh Token do banco e geramos novo Access Token com dados ATUAIS do usuário.
        // Não devemos confiar nas claims do token expirado exceto a identidade (Sub).
        
        // Então, eu vou retornar um Principal contendo apenas o Sub (Id).
        // Isso é suficiente para o RefreshTokenService identificar o usuário e buscar seus dados atuais no banco.
        
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
