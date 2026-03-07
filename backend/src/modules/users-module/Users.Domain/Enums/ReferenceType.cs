namespace Users.Domain.Enums;

/// <summary>
/// Tipo de referência para notificações (a qual entidade a notificação se refere).
/// </summary>
public enum ReferenceType
{
    /// <summary>
    /// Sem referência.
    /// </summary>
    Nenhum = 0,

    /// <summary>
    /// Referência a um pedido.
    /// </summary>
    Pedido = 1,

    /// <summary>
    /// Referência a um produto.
    /// </summary>
    Produto = 2,

    /// <summary>
    /// Referência a uma categoria.
    /// </summary>
    Categoria = 3,

    /// <summary>
    /// Referência a uma promoção.
    /// </summary>
    Promocao = 4,

    /// <summary>
    /// Referência a um cupom.
    /// </summary>
    Cupom = 5,

    /// <summary>
    /// Referência a um carrinho.
    /// </summary>
    Carrinho = 6,

    /// <summary>
    /// Referência a uma avaliação.
    /// </summary>
    Avaliacao = 7,

    /// <summary>
    /// Referência a uma conta de usuário.
    /// </summary>
    Conta = 8
}
