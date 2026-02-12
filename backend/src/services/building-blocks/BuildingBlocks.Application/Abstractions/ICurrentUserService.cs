namespace BuildingBlocks.Application.Abstractions;

/// <summary>
/// Interface para obter informações do usuário atual.
/// Usado para auditoria, autorização e contexto de negócio.
/// </summary>
public interface ICurrentUserService
{
    /// <summary>
    /// Identificador do usuário atual.
    /// Null se não autenticado.
    /// </summary>
    Guid? UserId { get; }

    /// <summary>
    /// Email do usuário atual.
    /// </summary>
    string? Email { get; }

    /// <summary>
    /// Nome de exibição do usuário atual.
    /// </summary>
    string? DisplayName { get; }

    /// <summary>
    /// Indica se o usuário está autenticado.
    /// </summary>
    bool IsAuthenticated { get; }

    /// <summary>
    /// Roles/papéis do usuário atual.
    /// </summary>
    IReadOnlyList<string> Roles { get; }

    /// <summary>
    /// Verifica se o usuário tem uma role específica.
    /// </summary>
    /// <param name="role">Nome da role.</param>
    /// <returns>True se o usuário possui a role.</returns>
    bool IsInRole(string role);

    /// <summary>
    /// Verifica se o usuário tem uma claim específica.
    /// </summary>
    /// <param name="claimType">Tipo da claim.</param>
    /// <param name="claimValue">Valor da claim (opcional).</param>
    /// <returns>True se o usuário possui a claim.</returns>
    bool HasClaim(string claimType, string? claimValue = null);

    /// <summary>
    /// Obtém o valor de uma claim específica.
    /// </summary>
    /// <param name="claimType">Tipo da claim.</param>
    /// <returns>Valor da claim ou null.</returns>
    string? GetClaimValue(string claimType);
}
