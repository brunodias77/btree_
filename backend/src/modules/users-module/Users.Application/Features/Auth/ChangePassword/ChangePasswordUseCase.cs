using Shared.Application.Abstractions;
using Shared.Application.Data;
using Shared.Application.Models;
using Users.Application.Services;
using Users.Domain.Errors;
using Users.Domain.Repositories;

namespace Users.Application.Features.Auth.ChangePassword;

public class ChangePasswordUseCase : IChangePasswordUseCase
{
    private readonly ICurrentUserService _currentUserService;
    private readonly IIdentityService _identityService;
    private readonly ISessionRepository _sessionRepository;
    private readonly IProfileRepository _profileRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly FluentValidation.IValidator<ChangePasswordInput> _validator;

    public ChangePasswordUseCase(
        ICurrentUserService currentUserService,
        IIdentityService identityService,
        ISessionRepository sessionRepository,
        IProfileRepository profileRepository,
        IUnitOfWork unitOfWork,
        FluentValidation.IValidator<ChangePasswordInput> validator)
    {
        _currentUserService = currentUserService;
        _identityService = identityService;
        _sessionRepository = sessionRepository;
        _profileRepository = profileRepository;
        _unitOfWork = unitOfWork;
        _validator = validator;
    }

    public async Task<Result<Result>> ExecuteAsync(ChangePasswordInput input, CancellationToken cancellationToken = default)
    {
        var validationResult = await _validator.ValidateAsync(input, cancellationToken);
        if (!validationResult.IsValid)
        {
            return Result.Failure<Result>(new Error("Validation.Error", validationResult.ToString()));
        }

        var userId = _currentUserService.UserId;
        if (!userId.HasValue)
        {
            return Result.Failure<Result>(AuthErrors.AcessoNaoAutorizado);
        }

        var changeResult = await _identityService.ChangePasswordAsync(userId.Value, input.CurrentPassword, input.NewPassword, cancellationToken);
        
        if (changeResult.IsFailure)
        {
            return Result.Failure<Result>(changeResult.Error);
        }

        // Revogar todas as sessões ativas do usuário para forçar re-login em outros dispositivos
        await _sessionRepository.RevokeAllUserSessionsAsync(userId.Value, cancellationToken);

        // Disparar o evento de domínio no Profile para ser capturado pelo Outbox
        var profile = await _profileRepository.GetByUserIdAsync(userId.Value, cancellationToken);
        if (profile is not null)
        {
            profile.RegisterPasswordChange();
            _profileRepository.Update(profile);
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(Result.Success());
    }
}
