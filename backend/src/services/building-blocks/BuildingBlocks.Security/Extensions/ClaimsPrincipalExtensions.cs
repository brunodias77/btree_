using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace BuildingBlocks.Security.Extensions;

/// <summary>
/// Extensões para ClaimsPrincipal.
/// Facilita acesso a informações do usuário autenticado.
/// </summary>
public static class ClaimsPrincipalExtensions
{
    /// <summary>
    /// Obtém o ID do usuário (subject).
    /// </summary>
    public static Guid? GetUserId(this ClaimsPrincipal principal)
    {
        var claim = principal.FindFirst(JwtRegisteredClaimNames.Sub)
            ?? principal.FindFirst(ClaimTypes.NameIdentifier);

        if (claim is null || !Guid.TryParse(claim.Value, out var userId))
            return null;

        return userId;
    }

    /// <summary>
    /// Obtém o ID do usuário ou lança exceção se não autenticado.
    /// </summary>
    public static Guid GetRequiredUserId(this ClaimsPrincipal principal)
    {
        return principal.GetUserId()
            ?? throw new InvalidOperationException("Usuário não autenticado.");
    }

    /// <summary>
    /// Obtém o email do usuário.
    /// </summary>
    public static string? GetEmail(this ClaimsPrincipal principal)
    {
        return principal.FindFirst(JwtRegisteredClaimNames.Email)?.Value
            ?? principal.FindFirst(ClaimTypes.Email)?.Value;
    }

    /// <summary>
    /// Obtém o nome de exibição do usuário.
    /// </summary>
    public static string? GetDisplayName(this ClaimsPrincipal principal)
    {
        return principal.FindFirst("display_name")?.Value
            ?? principal.FindFirst(ClaimTypes.Name)?.Value
            ?? principal.GetEmail();
    }

    /// <summary>
    /// Obtém as roles do usuário.
    /// </summary>
    public static IReadOnlyList<string> GetRoles(this ClaimsPrincipal principal)
    {
        return principal.FindAll(ClaimTypes.Role)
            .Select(c => c.Value)
            .ToList();
    }

    /// <summary>
    /// Verifica se o usuário tem uma permissão específica.
    /// </summary>
    public static bool HasPermission(this ClaimsPrincipal principal, string permission)
    {
        // Admin full access tem todas as permissões
        if (principal.HasClaim(Authorization.Permissions.ClaimType, Authorization.Permissions.Admin.FullAccess))
            return true;

        return principal.HasClaim(Authorization.Permissions.ClaimType, permission);
    }

    /// <summary>
    /// Verifica se o usuário é admin.
    /// </summary>
    public static bool IsAdmin(this ClaimsPrincipal principal)
    {
        return principal.IsInRole("Admin")
            || principal.HasClaim(Authorization.Permissions.ClaimType, Authorization.Permissions.Admin.FullAccess);
    }

    /// <summary>
    /// Obtém o valor de uma claim específica.
    /// </summary>
    public static string? GetClaimValue(this ClaimsPrincipal principal, string claimType)
    {
        return principal.FindFirst(claimType)?.Value;
    }

    /// <summary>
    /// Obtém todos os valores de um tipo de claim.
    /// </summary>
    public static IEnumerable<string> GetClaimValues(this ClaimsPrincipal principal, string claimType)
    {
        return principal.FindAll(claimType).Select(c => c.Value);
    }

    /// <summary>
    /// Verifica se o token é válido (não expirado).
    /// </summary>
    public static bool IsTokenValid(this ClaimsPrincipal principal)
    {
        var expClaim = principal.FindFirst(JwtRegisteredClaimNames.Exp);
        if (expClaim is null || !long.TryParse(expClaim.Value, out var exp))
            return false;

        var expirationTime = DateTimeOffset.FromUnixTimeSeconds(exp).UtcDateTime;
        return DateTime.UtcNow < expirationTime;
    }
}
