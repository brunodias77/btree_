using Shared.Application.UseCases;

namespace Users.Application.Features.Auth.Register;

public interface IRegisterUserUseCase : IUseCase<RegisterUserInput, Guid>
{
    
}