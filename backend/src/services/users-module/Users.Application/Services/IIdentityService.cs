using BuildingBlocks.Application.Models;
using Users.Domain.Identity;

namespace Users.Application.Services;

/// <summary>
/// Serviço de identidade para operações com ASP.NET Core Identity.
/// </summary>
public interface IIdentityService
{
    /// <summary>
    /// Verifica se o e-mail é único no sistema.
    /// </summary>
    Task<bool> IsEmailUniqueAsync(string email, CancellationToken cancellationToken = default);

    /// <summary>
    /// Cria um novo usuário no sistema de identidade.
    /// </summary>
    Task<Result<Guid>> CreateUserAsync(string email, string password, CancellationToken cancellationToken = default);

    /// <summary>
    /// Valida as credenciais do usuário.
    /// </summary>
    /// <param name="email">Email do usuário.</param>
    /// <param name="password">Senha do usuário.</param>
    /// <param name="cancellationToken">Token de cancelamento.</param>
    /// <returns>O usuário se credenciais válidas; caso contrário, erro.</returns>
    Task<Result<ApplicationUser>> ValidateCredentialsAsync(string email, string password, CancellationToken cancellationToken = default);

    /// <summary>
    /// Obtém um usuário pelo ID.
    /// </summary>
    /// <param name="userId">ID do usuário.</param>
    /// <param name="cancellationToken">Token de cancelamento.</param>
    /// <returns>O usuário se encontrado; caso contrário, null.</returns>
    Task<ApplicationUser?> GetByIdAsync(Guid userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Obtém um usuário pelo email.
    /// </summary>
    /// <param name="email">Email do usuário.</param>
    /// <param name="cancellationToken">Token de cancelamento.</param>
    /// <returns>O usuário se encontrado; caso contrário, null.</returns>
    Task<ApplicationUser?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);

    /// <summary>
    /// Confirma o email do usuário.
    /// </summary>
    /// <param name="userId">ID do usuário.</param>
    /// <param name="token">Token de confirmação.</param>
    /// <param name="cancellationToken">Token de cancelamento.</param>
    /// <returns>Resultado da operação.</returns>
    Task<Result> ConfirmEmailAsync(Guid userId, string token, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gera um token para redefinição de senha.
    /// </summary>
    /// <param name="email">Email do usuário.</param>
    /// <param name="cancellationToken">Token de cancelamento.</param>
    /// <returns>O token de redefinição se usuário existe; caso contrário, erro.</returns>
    Task<Result<string>> GeneratePasswordResetTokenAsync(string email, CancellationToken cancellationToken = default);

    /// <summary>
    /// Redefine a senha do usuário.
    /// </summary>
    /// <param name="email">Email do usuário.</param>
    /// <param name="token">Token de redefinição.</param>
    /// <param name="newPassword">Nova senha.</param>
    /// <param name="cancellationToken">Token de cancelamento.</param>
    /// <returns>Resultado da operação.</returns>
    Task<Result> ResetPasswordAsync(string email, string token, string newPassword, CancellationToken cancellationToken = default);

    /// <summary>
    /// Altera a senha do usuário.
    /// </summary>
    /// <param name="userId">ID do usuário.</param>
    /// <param name="currentPassword">Senha atual.</param>
    /// <param name="newPassword">Nova senha.</param>
    /// <param name="cancellationToken">Token de cancelamento.</param>
    /// <returns>Resultado da operação.</returns>
    Task<Result> ChangePasswordAsync(Guid userId, string currentPassword, string newPassword, CancellationToken cancellationToken = default);

    /// <summary>
    /// Verifica se o email já está em uso.
    /// </summary>
    /// <param name="email">Email a verificar.</param>
    /// <param name="cancellationToken">Token de cancelamento.</param>
    /// <returns>True se o email já existe; caso contrário, false.</returns>
    Task<bool> EmailExistsAsync(string email, CancellationToken cancellationToken = default);

    /// <summary>
    /// Obtém as roles de um usuário.
    /// </summary>
    /// <param name="userId">ID do usuário.</param>
    /// <param name="cancellationToken">Token de cancelamento.</param>
    /// <returns>Lista de roles do usuário.</returns>
    Task<IList<string>> GetRolesAsync(Guid userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Adiciona uma role a um usuário.
    /// </summary>
    /// <param name="userId">ID do usuário.</param>
    /// <param name="role">Nome da role.</param>
    /// <param name="cancellationToken">Token de cancelamento.</param>
    /// <returns>Resultado da operação.</returns>
    Task<Result> AddToRoleAsync(Guid userId, string role, CancellationToken cancellationToken = default);

    /// <summary>
    /// Remove uma role de um usuário.
    /// </summary>
    /// <param name="userId">ID do usuário.</param>
    /// <param name="role">Nome da role.</param>
    /// <param name="cancellationToken">Token de cancelamento.</param>
    /// <returns>Resultado da operação.</returns>
    Task<Result> RemoveFromRoleAsync(Guid userId, string role, CancellationToken cancellationToken = default);

    /// <summary>
    /// Deleta um usuário.
    /// </summary>
    /// <param name="userId">ID do usuário.</param>
    /// <param name="cancellationToken">Token de cancelamento.</param>
    /// <returns>Resultado da operação.</returns>
    Task<Result> DeleteUserAsync(Guid userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gera um token de confirmação de email para o usuário.
    /// </summary>
    /// <param name="userId">ID do usuário.</param>
    /// <param name="cancellationToken">Token de cancelamento.</param>
    /// <returns>O token de confirmação de email.</returns>
    Task<Result<string>> GenerateEmailConfirmationTokenAsync(Guid userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Garante que uma role existe no sistema. Cria se não existir.
    /// </summary>
    /// <param name="roleName">Nome da role.</param>
    /// <param name="cancellationToken">Token de cancelamento.</param>
    /// <returns>Resultado da operação.</returns>
    Task<Result> EnsureRoleExistsAsync(string roleName, CancellationToken cancellationToken = default);

    /// <summary>
    /// Verifica se o e-mail do usuário já foi confirmado.
    /// </summary>
    /// <param name="userId">ID do usuário.</param>
    /// <param name="cancellationToken">Token de cancelamento.</param>
    /// <returns>True se o e-mail já foi confirmado; caso contrário, false.</returns>
    Task<bool> IsEmailConfirmedAsync(Guid userId, CancellationToken cancellationToken = default);
}

