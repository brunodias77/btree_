using BuildingBlocks.Application.Models;

namespace Users.Domain.Errors;

/// <summary>
/// Erros relacionados a notificações.
/// </summary>
public static class NotificationErrors
{
    public static Error NaoEncontrada(Guid notificationId) =>
        Error.NotFound("Notification.NaoEncontrada", $"Notificação com ID '{notificationId}' não foi encontrada.");

    public static readonly Error NotificacaoNaoPertenceAoUsuario =
        Error.Forbidden("Notification.NaoPertenceAoUsuario", "Esta notificação não pertence ao usuário.");

    public static readonly Error JaLida =
        Error.Validation("Notification.JaLida", "Esta notificação já foi marcada como lida.");

    public static readonly Error TituloObrigatorio =
        Error.Validation("Notification.TituloObrigatorio", "O título da notificação é obrigatório.");

    public static readonly Error MensagemObrigatoria =
        Error.Validation("Notification.MensagemObrigatoria", "A mensagem da notificação é obrigatória.");

    public static readonly Error TipoObrigatorio =
        Error.Validation("Notification.TipoObrigatorio", "O tipo da notificação é obrigatório.");

    public static readonly Error PreferenciaNaoEncontrada =
        Error.NotFound("Notification.PreferenciaNaoEncontrada", "Preferências de notificação não encontradas para este usuário.");

    public static readonly Error CanalDesabilitado =
        Error.Validation("Notification.CanalDesabilitado", "O canal de notificação está desabilitado nas preferências do usuário.");
}
