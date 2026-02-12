
using BuildingBlocks.Application.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Users.Domain.Identity;
using BuildingBlocks.Application.Abstractions;
using Users.Application.Services;

namespace Users.Infrastructure.Services;


/// <summary>
/// Implementação do serviço de identidade usando ASP.NET Core Identity.
/// </summary>
public class IdentityService : IIdentityService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly RoleManager<ApplicationRole> _roleManager;
    private readonly ITokenService _tokenService; // Usado para gerar tokens de confirmação (se não for nativo do Identity)
    private readonly IEmailService _emailService;

    public IdentityService(
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager,
        RoleManager<ApplicationRole> roleManager,
        ITokenService tokenService,
        IEmailService emailService)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _roleManager = roleManager;
        _tokenService = tokenService;
        _emailService = emailService;
    }

    public async Task<bool> IsEmailUniqueAsync(string email, CancellationToken cancellationToken = default)
    {
        return !await EmailExistsAsync(email, cancellationToken);
    }

    public async Task<Result<Guid>> CreateUserAsync(string email, string password, CancellationToken cancellationToken = default)
    {
        var user = new ApplicationUser
        {
            UserName = email,
            Email = email
        };

        var result = await _userManager.CreateAsync(user, password);

        if (!result.Succeeded)
        {
            var errors = result.Errors.Select(e => new Error(e.Code, e.Description));
            return Result.Failure<Guid>(new Error("Identity.RegisterFailure", string.Join("; ", errors.Select(e => e.Message))));
        }

        // Adicionar role padrão "Customer"
        await EnsureRoleExistsAsync("Customer", cancellationToken);
        await _userManager.AddToRoleAsync(user, "Customer");

        // Gerar token de confirmação de email
        var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);

        // Enviar email de confirmação com link completo
        var confirmationLink = $"https://bcommerce.com/confirm-email?userId={user.Id}&token={Uri.EscapeDataString(token)}";
        await _emailService.SendEmailAsync(
            email, 
            "Confirmação de cadastro - BCommerce", 
            $"Olá! Para confirmar seu cadastro, clique no link abaixo:\n\n{confirmationLink}\n\nOu use os dados manualmente:\n- UserId: {user.Id}\n- Token: {token}"
        );

        return user.Id;
    }

    public async Task<Result<ApplicationUser>> ValidateCredentialsAsync(string email, string password, CancellationToken cancellationToken = default)
    {
        var user = await _userManager.FindByEmailAsync(email);
        if (user is null)
        {
            return Result.Failure<ApplicationUser>(new Error("Identity.UserNotFound", "Usuário ou senha inválidos."));
        }

        var result = await _signInManager.CheckPasswordSignInAsync(user, password, lockoutOnFailure: true);
        
        if (result.IsLockedOut)
        {
            return Result.Failure<ApplicationUser>(new Error("Identity.LockedOut", "Conta bloqueada temporariamente. Tente novamente mais tarde."));
        }
        
        if (!result.Succeeded)
        {
            return Result.Failure<ApplicationUser>(new Error("Identity.InvalidCredentials", "Usuário ou senha inválidos."));
        }

        return user;
    }

    public async Task<ApplicationUser?> GetByIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await _userManager.FindByIdAsync(userId.ToString());
    }

    public async Task<ApplicationUser?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        return await _userManager.FindByEmailAsync(email);
    }

    public async Task<Result> ConfirmEmailAsync(Guid userId, string token, CancellationToken cancellationToken = default)
    {
        var user = await _userManager.FindByIdAsync(userId.ToString());
        if (user is null)
        {
            return Result.Failure(new Error("Identity.UserNotFound", "Usuário não encontrado."));
        }

        var result = await _userManager.ConfirmEmailAsync(user, token);
        if (!result.Succeeded)
        {
            return Result.Failure(new Error("Identity.ConfirmEmailFailed", "Falha ao confirmar email. O link pode ser inválido ou ter expirado."));
        }

        return Result.Success();
    }

    public async Task<Result<string>> GeneratePasswordResetTokenAsync(string email, CancellationToken cancellationToken = default)
    {
        var user = await _userManager.FindByEmailAsync(email);
        if (user is null)
        {
            return Result.Failure<string>(new Error("Identity.UserNotFound", "Usuário não encontrado."));
        }

        var token = await _userManager.GeneratePasswordResetTokenAsync(user);
        return token;
    }

    public async Task<Result> ResetPasswordAsync(string email, string token, string newPassword, CancellationToken cancellationToken = default)
    {
        var user = await _userManager.FindByEmailAsync(email);
        if (user is null)
        {
            // Por segurança não revelamos que usuário não existe
            return Result.Success(); 
        }

        var result = await _userManager.ResetPasswordAsync(user, token, newPassword);
        if (!result.Succeeded)
        {
            var errors = string.Join("; ", result.Errors.Select(e => e.Description));
            return Result.Failure(new Error("Identity.ResetPasswordFailed", $"Falha ao redefinir senha: {errors}"));
        }

        return Result.Success();
    }

    public async Task<Result> ChangePasswordAsync(Guid userId, string currentPassword, string newPassword, CancellationToken cancellationToken = default)
    {
        var user = await _userManager.FindByIdAsync(userId.ToString());
        if (user is null)
        {
            return Result.Failure(new Error("Identity.UserNotFound", "Usuário não encontrado."));
        }

        var result = await _userManager.ChangePasswordAsync(user, currentPassword, newPassword);
        if (!result.Succeeded)
        {
            var errors = string.Join("; ", result.Errors.Select(e => e.Description));
            return Result.Failure(new Error("Identity.ChangePasswordFailed", $"Falha ao alterar senha: {errors}"));
        }

        return Result.Success();
    }

    public async Task<bool> EmailExistsAsync(string email, CancellationToken cancellationToken = default)
    {
        return await _userManager.Users.AnyAsync(u => u.Email == email, cancellationToken);
    }

    public async Task<IList<string>> GetRolesAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var user = await _userManager.FindByIdAsync(userId.ToString());
        if (user is null) return new List<string>();

        return await _userManager.GetRolesAsync(user);
    }

    public async Task<Result> AddToRoleAsync(Guid userId, string role, CancellationToken cancellationToken = default)
    {
        var user = await _userManager.FindByIdAsync(userId.ToString());
        if (user is null) return Result.Failure(new Error("Identity.UserNotFound", "Usuário não encontrado."));

        await EnsureRoleExistsAsync(role, cancellationToken);

        var result = await _userManager.AddToRoleAsync(user, role);
        return result.Succeeded ? Result.Success() : Result.Failure(new Error("Identity.AddRoleFailed", "Falha ao adicionar role."));
    }

    public async Task<Result> RemoveFromRoleAsync(Guid userId, string role, CancellationToken cancellationToken = default)
    {
        var user = await _userManager.FindByIdAsync(userId.ToString());
        if (user is null) return Result.Failure(new Error("Identity.UserNotFound", "Usuário não encontrado."));

        var result = await _userManager.RemoveFromRoleAsync(user, role);
        return result.Succeeded ? Result.Success() : Result.Failure(new Error("Identity.RemoveRoleFailed", "Falha ao remover role."));
    }

    public async Task<Result> DeleteUserAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var user = await _userManager.FindByIdAsync(userId.ToString());
        if (user is null) return Result.Failure(new Error("Identity.UserNotFound", "Usuário não encontrado."));

        var result = await _userManager.DeleteAsync(user);
        return result.Succeeded ? Result.Success() : Result.Failure(new Error("Identity.DeleteUserFailed", "Falha ao deletar usuário."));
    }

    public async Task<Result<string>> GenerateEmailConfirmationTokenAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var user = await _userManager.FindByIdAsync(userId.ToString());
        if (user is null) return Result.Failure<string>(new Error("Identity.UserNotFound", "Usuário não encontrado."));

        return await _userManager.GenerateEmailConfirmationTokenAsync(user);
    }

    public async Task<Result> EnsureRoleExistsAsync(string roleName, CancellationToken cancellationToken = default)
    {
        if (!await _roleManager.RoleExistsAsync(roleName))
        {
            var role = new ApplicationRole { Name = roleName };
            var result = await _roleManager.CreateAsync(role);
            if (!result.Succeeded) return Result.Failure(new Error("Identity.CreateRoleFailed", "Falha ao criar role."));
        }
        return Result.Success();
    }

    public async Task<bool> IsEmailConfirmedAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var user = await _userManager.FindByIdAsync(userId.ToString());
        if (user is null) return false;

        return user.EmailConfirmed;
    }
}

