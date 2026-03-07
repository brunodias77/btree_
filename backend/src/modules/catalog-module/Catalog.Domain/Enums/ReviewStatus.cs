namespace Catalog.Domain.Enums;

/// <summary>
/// Status de aprovação de uma avaliação de produto.
/// </summary>
public enum ReviewStatus
{
    /// <summary>
    /// Avaliação pendente de moderação.
    /// </summary>
    Pending = 0,

    /// <summary>
    /// Avaliação aprovada e visível.
    /// </summary>
    Approved = 1,

    /// <summary>
    /// Avaliação rejeitada por violar regras.
    /// </summary>
    Rejected = 2
}
