using Shared.Application.Models;
using Shared.Application.UseCases;

namespace Users.Application.Features.Admin.AdminLogin;

public interface IAdminLoginUserUseCase : IUseCase<AdminLoginUserInput, AdminLoginUserOutput>
{
    
}