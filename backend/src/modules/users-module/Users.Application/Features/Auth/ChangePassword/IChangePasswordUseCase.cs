using Shared.Application.Models;
using Shared.Application.UseCases;

namespace Users.Application.Features.Auth.ChangePassword;

public interface IChangePasswordUseCase : IUseCase<ChangePasswordInput, Result>
{
}
