namespace Users.Domain.Enums;

/// <summary>
/// Tipo de notificação do usuário.
/// </summary>
public enum NotificationType
{
    /// <summary>
    /// Informação geral.
    /// </summary>
    Informacao = 0,

    /// <summary>
    /// Atualização de pedido.
    /// </summary>
    AtualizacaoPedido = 1,

    /// <summary>
    /// Promoção ou oferta.
    /// </summary>
    Promocao = 2,

    /// <summary>
    /// Queda de preço.
    /// </summary>
    QuedaPreco = 3,

    /// <summary>
    /// Produto de volta ao estoque.
    /// </summary>
    VoltouEstoque = 4,

    /// <summary>
    /// Avaliação de produto.
    /// </summary>
    AvaliacaoProduto = 5,

    /// <summary>
    /// Alerta de segurança.
    /// </summary>
    AlertaSeguranca = 6,

    /// <summary>
    /// Atualização de conta.
    /// </summary>
    AtualizacaoConta = 7,

    /// <summary>
    /// Newsletter.
    /// </summary>
    Newsletter = 8,

    /// <summary>
    /// Lembrete de carrinho abandonado.
    /// </summary>
    CarrinhoAbandonado = 9
}
