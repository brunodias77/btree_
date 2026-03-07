using Shared.Application.Data;
using Shared.Application.Models;
using Users.Application.Services;
using Users.Domain.Errors;
using Users.Domain.Repositories;

namespace Users.Application.Features.Auth.ResetPassword;

public class ResetPasswordUseCase : IResetPasswordUseCase
{
    private readonly IIdentityService _identityService;
    private readonly ISessionRepository _sessionRepository;
    private readonly IProfileRepository _profileRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly FluentValidation.IValidator<ResetPasswordInput> _validator;

    public ResetPasswordUseCase(
        IIdentityService identityService,
        ISessionRepository sessionRepository,
        IProfileRepository profileRepository,
        IUnitOfWork unitOfWork,
        FluentValidation.IValidator<ResetPasswordInput> validator)
    {
        _identityService = identityService;
        _sessionRepository = sessionRepository;
        _profileRepository = profileRepository;
        _unitOfWork = unitOfWork;
        _validator = validator;
    }

    public async Task<Result<Result>> ExecuteAsync(ResetPasswordInput input, CancellationToken cancellationToken = default)
    {
        var validationResult = await _validator.ValidateAsync(input, cancellationToken);
        if (!validationResult.IsValid)
        {
            return Result.Failure<Result>(new Error("Validation.Error", validationResult.ToString()));
        }

        var profile = await _profileRepository.GetByPasswordResetCodeAsync(input.Code, cancellationToken);
        if (profile is null || !profile.VerifyPasswordResetCode(input.Code))
        {
            return Result.Failure<Result>(AuthErrors.TokenInvalido);
        }

        var user = await _identityService.GetByIdAsync(profile.UserId, cancellationToken);
        if (user is null)
        {
            // Segurança: Se o usuário não existe, retornar o mesmo erro
            return Result.Failure<Result>(AuthErrors.TokenInvalido);
        }

        // Tenta redefinir a senha através do Identity
        var resetResult = await _identityService.ResetPasswordAsync(user.Email!, profile.PasswordResetToken!, input.NewPassword, cancellationToken);
        
        if (resetResult.IsFailure)
        {
            return Result.Failure<Result>(resetResult.Error);
        }

        // Revogar todas as sessões ativas do usuário
        await _sessionRepository.RevokeAllUserSessionsAsync(user.Id, cancellationToken);

        profile.ConfirmPasswordReset();
        _profileRepository.Update(profile);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(Result.Success());
    }
}
