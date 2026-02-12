using Microsoft.AspNetCore.Authorization;

namespace BuildingBlocks.Security.Authorization;

/// <summary>
/// Requirement de autorização baseado em permissão.
/// Usado com policies de autorização do ASP.NET Core.
/// </summary>
public sealed class PermissionRequirement : IAuthorizationRequirement
{
    /// <summary>
    /// Permissão requerida.
    /// </summary>
    public string Permission { get; }

    public PermissionRequirement(string permission)
    {
        Permission = permission;
    }
}

/// <summary>
/// Attribute para aplicar requisito de permissão em controllers/actions.
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
public sealed class RequirePermissionAttribute : AuthorizeAttribute
{
    /// <summary>
    /// Permissão requerida.
    /// </summary>
    public string Permission { get; }

    public RequirePermissionAttribute(string permission)
        : base(policy: permission)
    {
        Permission = permission;
    }
}
