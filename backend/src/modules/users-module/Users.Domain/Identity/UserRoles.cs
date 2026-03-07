namespace Users.Domain.Identity;

/// <summary>
/// Constantes para roles do sistema.
/// </summary>
public static class UserRoles
{
    /// <summary>
    /// Administrador do sistema com acesso total.
    /// </summary>
    public const string Admin = "Admin";
    
    /// <summary>
    /// Cliente padrão da loja.
    /// </summary>
    public const string Customer = "Customer";
    
    /// <summary>
    /// Vendedor/Lojista.
    /// </summary>
    public const string Seller = "Seller";
    
    /// <summary>
    /// Retorna todas as roles disponíveis.
    /// </summary>
    public static IReadOnlyList<string> All => new[] { Admin, Customer, Seller };
}