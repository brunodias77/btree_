using Shared.Application.Models;
using Shared.Application.UseCases;

namespace Users.Application.Features.Auth.Logout;

public interface ILogoutUseCase : IUseCase<LogoutInput, Result>
{
}
