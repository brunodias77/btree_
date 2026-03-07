using Shared.Application.Data;
using Shared.Application.Models;
using Shared.Security.Abstractions;
using Users.Application.Services;
using Users.Domain.Repositories;

namespace Users.Infrastructure.Services;

public class RefreshTokenService : IRefreshTokenService
{
    private readonly ISessionRepository _sessionRepository;
    private readonly ITokenGenerator _tokenService;
    private readonly IIdentityService _identityService;
    private readonly System.Security.Cryptography.SHA256 _sha256;
    private readonly IUnitOfWork _unitOfWork; // Added IUnitOfWork

    public RefreshTokenService(
        ISessionRepository sessionRepository,
        ITokenGenerator tokenService,
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
        var extractedUserId = _tokenService.GetUserIdFromExpiredToken(accessToken);
        
        if (extractedUserId == null)
        {
            return Result.Failure<(string, string)>(new Error("Auth.InvalidToken", "Token de acesso inválido."));
        }
        var userId = extractedUserId.Value;

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
        
        var tokenResult = _tokenService.GenerateAccessToken(user.Id, user.Email!, roles);
        var newAccessToken = tokenResult.Token;
        
        var refreshTokenResult = _tokenService.GenerateRefreshToken();
        
        // 6. Rotacionar Refresh Token na Sessão
        session.UpdateRefreshToken(refreshTokenResult.TokenHash, refreshTokenResult.ExpiresAt);
        
        _sessionRepository.Update(session);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return (newAccessToken, refreshTokenResult.Token);
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
