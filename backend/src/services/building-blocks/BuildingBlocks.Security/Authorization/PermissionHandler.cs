using Microsoft.AspNetCore.Authorization;

namespace BuildingBlocks.Security.Authorization;

/// <summary>
/// Handler de autorização para verificar permissões.
/// Verifica se o usuário possui a claim de permissão requerida.
/// </summary>
public sealed class PermissionHandler : AuthorizationHandler<PermissionRequirement>
{
    protected override Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        PermissionRequirement requirement)
    {
        // Verifica se o usuário está autenticado
        if (context.User.Identity?.IsAuthenticated != true)
        {
            return Task.CompletedTask;
        }

        // Verifica se tem permissão de admin full access (bypass)
        if (context.User.HasClaim(Permissions.ClaimType, Permissions.Admin.FullAccess))
        {
            context.Succeed(requirement);
            return Task.CompletedTask;
        }

        // Verifica se tem a permissão específica
        if (context.User.HasClaim(Permissions.ClaimType, requirement.Permission))
        {
            context.Succeed(requirement);
        }

        return Task.CompletedTask;
    }
}

/// <summary>
/// Provider de policies de autorização.
/// Cria policies dinamicamente baseadas em permissões.
/// </summary>
public sealed class PermissionPolicyProvider : IAuthorizationPolicyProvider
{
    private readonly DefaultAuthorizationPolicyProvider _fallbackProvider;

    public PermissionPolicyProvider(Microsoft.Extensions.Options.IOptions<AuthorizationOptions> options)
    {
        _fallbackProvider = new DefaultAuthorizationPolicyProvider(options);
    }

    /// <summary>
    /// Retorna a policy padrão.
    /// </summary>
    public Task<AuthorizationPolicy> GetDefaultPolicyAsync()
    {
        return _fallbackProvider.GetDefaultPolicyAsync();
    }

    /// <summary>
    /// Retorna a policy fallback.
    /// </summary>
    public Task<AuthorizationPolicy?> GetFallbackPolicyAsync()
    {
        return _fallbackProvider.GetFallbackPolicyAsync();
    }

    /// <summary>
    /// Retorna a policy pelo nome.
    /// Se não encontrar uma policy explícita, cria uma baseada em permissão.
    /// </summary>
    public async Task<AuthorizationPolicy?> GetPolicyAsync(string policyName)
    {
        // Tenta buscar policy existente
        var policy = await _fallbackProvider.GetPolicyAsync(policyName);

        if (policy is not null)
            return policy;

        // Se não encontrou, cria policy baseada em permissão
        // O nome da policy é a própria permissão
        return new AuthorizationPolicyBuilder()
            .AddRequirements(new PermissionRequirement(policyName))
            .Build();
    }
}
