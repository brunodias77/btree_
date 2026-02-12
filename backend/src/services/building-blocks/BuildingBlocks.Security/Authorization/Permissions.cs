namespace BuildingBlocks.Security.Authorization;

/// <summary>
/// Definição centralizada de permissões do sistema.
/// Mapeado para claim_type/claim_value em users.asp_net_user_claims e users.asp_net_role_claims.
/// </summary>
public static class Permissions
{
    /// <summary>
    /// Tipo da claim de permissão.
    /// </summary>
    public const string ClaimType = "permission";

    #region Módulo Users

    public static class Users
    {
        public const string View = "users:view";
        public const string Create = "users:create";
        public const string Update = "users:update";
        public const string Delete = "users:delete";
        public const string ManageRoles = "users:manage_roles";
    }

    #endregion

    #region Módulo Catalog

    public static class Catalog
    {
        public const string ViewProducts = "catalog:view_products";
        public const string CreateProduct = "catalog:create_product";
        public const string UpdateProduct = "catalog:update_product";
        public const string DeleteProduct = "catalog:delete_product";

        public const string ViewCategories = "catalog:view_categories";
        public const string ManageCategories = "catalog:manage_categories";

        public const string ManageStock = "catalog:manage_stock";
        public const string ViewStock = "catalog:view_stock";

        public const string ManageReviews = "catalog:manage_reviews";
    }

    #endregion

    #region Módulo Orders

    public static class Orders
    {
        public const string ViewOwn = "orders:view_own";
        public const string ViewAll = "orders:view_all";
        public const string Create = "orders:create";
        public const string UpdateStatus = "orders:update_status";
        public const string Cancel = "orders:cancel";
        public const string Refund = "orders:refund";
    }

    #endregion

    #region Módulo Payments

    public static class Payments
    {
        public const string View = "payments:view";
        public const string Process = "payments:process";
        public const string Refund = "payments:refund";
        public const string ViewTransactions = "payments:view_transactions";
    }

    #endregion

    #region Módulo Coupons

    public static class Coupons
    {
        public const string View = "coupons:view";
        public const string Create = "coupons:create";
        public const string Update = "coupons:update";
        public const string Delete = "coupons:delete";
        public const string Apply = "coupons:apply";
    }

    #endregion

    #region Módulo Cart

    public static class Cart
    {
        public const string ViewOwn = "cart:view_own";
        public const string ViewAll = "cart:view_all";
        public const string Manage = "cart:manage";
    }

    #endregion

    #region Administração

    public static class Admin
    {
        public const string Dashboard = "admin:dashboard";
        public const string Reports = "admin:reports";
        public const string Settings = "admin:settings";
        public const string AuditLogs = "admin:audit_logs";
        public const string FullAccess = "admin:full_access";
    }

    #endregion

    /// <summary>
    /// Retorna todas as permissões do sistema.
    /// Útil para seed de dados ou UI de administração.
    /// </summary>
    public static IEnumerable<string> GetAll()
    {
        yield return Users.View;
        yield return Users.Create;
        yield return Users.Update;
        yield return Users.Delete;
        yield return Users.ManageRoles;

        yield return Catalog.ViewProducts;
        yield return Catalog.CreateProduct;
        yield return Catalog.UpdateProduct;
        yield return Catalog.DeleteProduct;
        yield return Catalog.ViewCategories;
        yield return Catalog.ManageCategories;
        yield return Catalog.ManageStock;
        yield return Catalog.ViewStock;
        yield return Catalog.ManageReviews;

        yield return Orders.ViewOwn;
        yield return Orders.ViewAll;
        yield return Orders.Create;
        yield return Orders.UpdateStatus;
        yield return Orders.Cancel;
        yield return Orders.Refund;

        yield return Payments.View;
        yield return Payments.Process;
        yield return Payments.Refund;
        yield return Payments.ViewTransactions;

        yield return Coupons.View;
        yield return Coupons.Create;
        yield return Coupons.Update;
        yield return Coupons.Delete;
        yield return Coupons.Apply;

        yield return Cart.ViewOwn;
        yield return Cart.ViewAll;
        yield return Cart.Manage;

        yield return Admin.Dashboard;
        yield return Admin.Reports;
        yield return Admin.Settings;
        yield return Admin.AuditLogs;
        yield return Admin.FullAccess;
    }
}
