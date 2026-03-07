using Shared.Application.Models;
using Users.Application.Services;

namespace Users.Application.Features.Auth.ConfirmEmail;

public class ConfirmEmailUseCase : IConfirmEmailUseCase
{
    private readonly IIdentityService _identityService;
    private readonly FluentValidation.IValidator<ConfirmEmailInput> _validator;

    public ConfirmEmailUseCase(
        IIdentityService identityService,
        FluentValidation.IValidator<ConfirmEmailInput> validator)
    {
        _identityService = identityService;
        _validator = validator;
    }

    public async Task<Result<Result>> ExecuteAsync(ConfirmEmailInput input, CancellationToken cancellationToken = default)
    {
        var validationResult = await _validator.ValidateAsync(input, cancellationToken);
        if (!validationResult.IsValid)
        {
            return Result.Failure<Result>(new Error("Validation.Error", validationResult.ToString()));
        }

        var result = await _identityService.ConfirmEmailAsync(input.Code, cancellationToken);
        if (result.IsFailure)
        {
            return Result.Failure<Result>(result.Error);
        }

        return Result.Success(Result.Success());
    }
}
