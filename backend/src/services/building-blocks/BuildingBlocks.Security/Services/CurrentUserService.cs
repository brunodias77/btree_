using System.Security.Claims;
using BuildingBlocks.Application.Abstractions;
using BuildingBlocks.Security.Extensions;
using Microsoft.AspNetCore.Http;

namespace BuildingBlocks.Security.Services;

/// <summary>
/// Implementação do serviço de usuário atual.
/// Obtém informações do usuário autenticado via HttpContext.
/// </summary>
public sealed class CurrentUserService : ICurrentUserService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CurrentUserService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    private ClaimsPrincipal? User => _httpContextAccessor.HttpContext?.User;

    /// <summary>
    /// Identificador do usuário atual.
    /// </summary>
    public Guid? UserId => User?.GetUserId();

    /// <summary>
    /// Email do usuário atual.
    /// </summary>
    public string? Email => User?.GetEmail();

    /// <summary>
    /// Nome de exibição do usuário atual.
    /// </summary>
    public string? DisplayName => User?.GetDisplayName();

    /// <summary>
    /// Indica se o usuário está autenticado.
    /// </summary>
    public bool IsAuthenticated => User?.Identity?.IsAuthenticated ?? false;

    /// <summary>
    /// Roles do usuário atual.
    /// </summary>
    public IReadOnlyList<string> Roles => User?.GetRoles() ?? Array.Empty<string>();

    /// <summary>
    /// Verifica se o usuário tem uma role específica.
    /// </summary>
    public bool IsInRole(string role) => User?.IsInRole(role) ?? false;

    /// <summary>
    /// Verifica se o usuário tem uma claim específica.
    /// </summary>
    public bool HasClaim(string claimType, string? claimValue = null)
    {
        if (User is null)
            return false;

        if (claimValue is null)
            return User.HasClaim(c => c.Type == claimType);

        return User.HasClaim(claimType, claimValue);
    }

    /// <summary>
    /// Obtém o valor de uma claim específica.
    /// </summary>
    public string? GetClaimValue(string claimType)
    {
        return User?.FindFirst(claimType)?.Value;
    }
}
