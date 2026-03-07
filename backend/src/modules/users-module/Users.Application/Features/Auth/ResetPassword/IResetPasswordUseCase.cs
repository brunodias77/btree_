using Shared.Application.Models;
using Shared.Application.UseCases;

namespace Users.Application.Features.Auth.ResetPassword;

public interface IResetPasswordUseCase : IUseCase<ResetPasswordInput, Result>
{
}
