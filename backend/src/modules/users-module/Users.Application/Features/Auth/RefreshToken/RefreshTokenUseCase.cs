using Shared.Application.Data;
using Shared.Application.Models;
using Shared.Security.Abstractions;
using Users.Application.Services;
using Users.Domain.Aggregates.Sessions;
using Users.Domain.Errors;
using Users.Domain.Repositories;

namespace Users.Application.Features.Auth.RefreshToken;

public class RefreshTokenUseCase : IRefreshTokenUseCase
{
    private readonly ISessionRepository _sessionRepository;
    private readonly IIdentityService _identityService;
    private readonly ITokenGenerator _tokenGenerator;
    private readonly IUnitOfWork _unitOfWork;
    private readonly FluentValidation.IValidator<RefreshTokenInput> _validator;

    public RefreshTokenUseCase(
        ISessionRepository sessionRepository,
        IIdentityService identityService,
        ITokenGenerator tokenGenerator,
        IUnitOfWork unitOfWork,
        FluentValidation.IValidator<RefreshTokenInput> validator)
    {
        _sessionRepository = sessionRepository;
        _identityService = identityService;
        _tokenGenerator = tokenGenerator;
        _unitOfWork = unitOfWork;
        _validator = validator;
    }

    public async Task<Result<RefreshTokenOutput>> ExecuteAsync(RefreshTokenInput input, CancellationToken cancellationToken = default)
    {
        // 1. Validator
        var validationResult = await _validator.ValidateAsync(input, cancellationToken);
        if (!validationResult.IsValid)
        {
            return Result.Failure<RefreshTokenOutput>(new Error("Validation.Error", validationResult.ToString()));
        }

        // 2. Refresh Token Hash setup
        // Assuming we need to verify the raw string or if it's stored directly
        // The instructions mentioned searching by the refresh token hash
        // We'll pass it exactly as the GetByRefreshTokenAsync expects
        var session = await _sessionRepository.GetByRefreshTokenAsync(input.RefreshToken, cancellationToken);

        if (session == null)
        {
            return Result.Failure<RefreshTokenOutput>(SessionErrors.RefreshTokenInvalido);
        }

        // 3. Validação de Segurança (Anti-Replay)
        if (session.IsRevoked())
        {
            // O token foi comprometido e está a ser reutilizado. Revocar todas as sessões ativas do utilizador.
            await _sessionRepository.RevokeAllUserSessionsAsync(session.UserId, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return Result.Failure<RefreshTokenOutput>(SessionErrors.SessaoRevogada);
        }

        if (session.IsExpired())
        {
            return Result.Failure<RefreshTokenOutput>(SessionErrors.RefreshTokenExpirado);
        }

        // 4. Recuperar o Utilizador
        var user = await _identityService.GetByIdAsync(session.UserId, cancellationToken);

        if (user == null || !user.EmailConfirmed || (user.LockoutEnd.HasValue && user.LockoutEnd.Value > DateTimeOffset.UtcNow))
        {
            return Result.Failure<RefreshTokenOutput>(AuthErrors.ContaBloqueada);
        }

        // 5. Revogar Sessão Atual
        session.Revoke("Rotated");
        _sessionRepository.Update(session);

        // 6. Geração de Novos Tokens
        var roles = await _identityService.GetRolesAsync(user.Id, cancellationToken);
        var newAccessToken = _tokenGenerator.GenerateAccessToken(user.Id, user.Email!, roles);
        var newRefreshToken = _tokenGenerator.GenerateRefreshToken();

        // 7. Criar Nova Sessão
        var newSession = Session.Create(
            userId: user.Id,
            refreshTokenHash: newRefreshToken.TokenHash,
            expiresAt: newRefreshToken.ExpiresAt,
            deviceId: session.DeviceId,
            deviceName: input.DeviceInfo ?? session.DeviceName,
            deviceType: session.DeviceType,
            ipAddress: input.IpAddress ?? session.IpAddress,
            country: session.Country,
            city: session.City,
            isCurrent: true
        );

        await _sessionRepository.AddAsync(newSession, cancellationToken);

        // 8. Persistência
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // 9. Dados de Retorno
        return Result.Success(new RefreshTokenOutput(
            AccessToken: newAccessToken.Token,
            RefreshToken: newRefreshToken.Token, // the raw token returned to the client
            ExpiresIn: newAccessToken.ExpiresIn
        ));
    }
}
