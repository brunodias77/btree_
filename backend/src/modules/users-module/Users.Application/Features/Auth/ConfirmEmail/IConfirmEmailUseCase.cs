using Shared.Application.Models;
using Shared.Application.UseCases;

namespace Users.Application.Features.Auth.ConfirmEmail;

public interface IConfirmEmailUseCase : IUseCase<ConfirmEmailInput, Result>
{
    
}