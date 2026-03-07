using Shared.Application.Models;
using Shared.Application.UseCases;

namespace Users.Application.Features.Auth.Login;

public interface ILoginUserUseCase : IUseCase<LoginUserInput, LoginUserOutput>
{
    
}