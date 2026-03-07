using Shared.Application.Models;
using Shared.Application.UseCases;

namespace Users.Application.Features.Auth.ResendConfirmationEmail;

public interface IResendConfirmationEmailUseCase : IUseCase<ResendConfirmationEmailInput, Result>
{
}
