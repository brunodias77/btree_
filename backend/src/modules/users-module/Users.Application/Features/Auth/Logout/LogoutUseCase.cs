using Shared.Application.Data;
using Shared.Application.Models;
using Users.Domain.Repositories;

namespace Users.Application.Features.Auth.Logout;

public class LogoutUseCase : ILogoutUseCase
{
    private readonly ISessionRepository _sessionRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly FluentValidation.IValidator<LogoutInput> _validator;

    public LogoutUseCase(
        ISessionRepository sessionRepository,
        IUnitOfWork unitOfWork,
        FluentValidation.IValidator<LogoutInput> validator)
    {
        _sessionRepository = sessionRepository;
        _unitOfWork = unitOfWork;
        _validator = validator;
    }

    public async Task<Result<Result>> ExecuteAsync(LogoutInput input, CancellationToken cancellationToken = default)
    {
        var validationResult = await _validator.ValidateAsync(input, cancellationToken);
        if (!validationResult.IsValid)
        {
            return Result.Failure<Result>(new Error("Validation.Error", validationResult.ToString()));
        }

        var session = await _sessionRepository.GetByRefreshTokenAsync(input.RefreshToken, cancellationToken);

        if (session is null || session.IsRevoked() || session.IsExpired())
        {
            return Result.Success(Result.Success());
        }

        session.Revoke("User Logout");

        _sessionRepository.Update(session);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(Result.Success());
    }
}
