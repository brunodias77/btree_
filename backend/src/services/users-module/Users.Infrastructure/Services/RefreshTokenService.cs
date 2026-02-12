
using BuildingBlocks.Application.Models;
using Microsoft.IdentityModel.Tokens;
using Users.Domain.Aggregates.Session;
using BuildingBlocks.Application.Data;
using Users.Application.Repositories;
using Users.Application.Services;

namespace Users.Infrastructure.Services;

/// <summary>
/// Implementação do serviço de Refresh Token.
/// </summary>
public class RefreshTokenService : IRefreshTokenService
{
    private readonly ISessionRepository _sessionRepository;
    private readonly ITokenService _tokenService;
    private readonly IIdentityService _identityService;
    private readonly System.Security.Cryptography.SHA256 _sha256;
    private readonly IUnitOfWork _unitOfWork; // Added IUnitOfWork

    public RefreshTokenService(
        ISessionRepository sessionRepository,
        ITokenService tokenService,
        IIdentityService identityService,
        IUnitOfWork unitOfWork) // Injected IUnitOfWork
    {
        _sessionRepository = sessionRepository;
        _tokenService = tokenService;
        _identityService = identityService;
        _unitOfWork = unitOfWork; // Initialized _unitOfWork
        _sha256 = System.Security.Cryptography.SHA256.Create();
    }

    public async Task<Result<(string AccessToken, string RefreshToken)>> RefreshAsync(string accessToken, string refreshToken, CancellationToken cancellationToken = default)
    {
        // 1. Obter ID do usuário do token expirado
        var principal = _tokenService.GetPrincipalFromExpiredToken(accessToken);
        var userIdClaim = principal.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        
        if (userIdClaim == null || !Guid.TryParse(userIdClaim, out var userId))
        {
            return Result.Failure<(string, string)>(new Error("Auth.InvalidToken", "Token de acesso inválido."));
        }

        // 2. Hash do refresh token recebido para buscar no banco
        var refreshTokenHash = HashRefreshToken(refreshToken);

        // 3. Buscar sessão
        var session = await _sessionRepository.GetByRefreshTokenAsync(refreshTokenHash, cancellationToken);
        
        if (session is null)
        {
            return Result.Failure<(string, string)>(new Error("Auth.InvalidRefreshToken", "Refresh token inválido ou não encontrado."));
        }

        // 4. Validar sessão (uso, expiração, revogação)
        // Nota: A entidade Session encapsula regras de negócio, mas aqui validamos se corresponde ao usuário
        if (session.UserId != userId)
        {
             return Result.Failure<(string, string)>(new Error("Auth.InvalidRefreshToken", "Refresh token não pertence ao usuário."));
        }

        if (session.IsRevoked())
        {
            // Padrão de segurança: se tentar usar token revogado, revogar tudo (possível roubo)
            await _sessionRepository.RevokeAllUserSessionsAsync(userId, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return Result.Failure<(string, string)>(new Error("Auth.RevokedToken", "Token revogado. Sessão encerrada."));
        }

        if (session.ExpiresAt <= DateTime.UtcNow)
        {
            return Result.Failure<(string, string)>(new Error("Auth.ExpiredSession", "Sessão expirada. Faça login novamente."));
        }

        // 5. Gerar novos tokens
        var user = await _identityService.GetByIdAsync(userId, cancellationToken);
        if (user is null)
        {
            return Result.Failure<(string, string)>(new Error("Auth.UserNotFound", "Usuário não encontrado."));
        }
        
        var roles = await _identityService.GetRolesAsync(userId, cancellationToken);
        
        // Montar claims
        var claims = new List<System.Security.Claims.Claim>
        {
            new(System.Security.Claims.ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(System.Security.Claims.ClaimTypes.Email, user.Email!)
        };
        claims.AddRange(roles.Select(r => new System.Security.Claims.Claim(System.Security.Claims.ClaimTypes.Role, r)));

        var newAccessToken = _tokenService.GenerateAccessToken(claims);
        var (newRefreshToken, newRefreshTokenHash) = _tokenService.GenerateRefreshToken();
        
        // 6. Rotacionar Refresh Token na Sessão
        var newExpiry = DateTime.UtcNow.AddDays(7); // TODO: Obter de configuração/serviço

        session.UpdateRefreshToken(newRefreshTokenHash, newExpiry);
        
        _sessionRepository.Update(session);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return (newAccessToken, newRefreshToken);
    }

    public async Task<Result> RevokeAsync(string refreshToken, CancellationToken cancellationToken = default)
    {
        var refreshTokenHash = HashRefreshToken(refreshToken);
        var session = await _sessionRepository.GetByRefreshTokenAsync(refreshTokenHash, cancellationToken);
        if (session != null)
        {
            session.Revoke();
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }
        return Result.Success();
    }
    
    private string HashRefreshToken(string token)
    {
        var hashBytes = _sha256.ComputeHash(System.Text.Encoding.UTF8.GetBytes(token));
        return Convert.ToBase64String(hashBytes);
    }
}
