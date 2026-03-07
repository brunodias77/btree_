using Shared.Application.Models;
using Shared.Application.UseCases;

namespace Users.Application.Features.Auth.ForgotPassword;

public interface IForgotPasswordUseCase : IUseCase<ForgotPasswordInput, Result>
{
}
