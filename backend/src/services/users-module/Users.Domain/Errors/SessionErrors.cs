using BuildingBlocks.Application.Models;

namespace Users.Domain.Errors;

/// <summary>
/// Erros relacionados a sessões.
/// </summary>
public static class SessionErrors
{
    public static Error NaoEncontrada(Guid sessionId) =>
        Error.NotFound("Session.NaoEncontrada", $"Sessão com ID '{sessionId}' não foi encontrada.");

    public static readonly Error RefreshTokenInvalido =
        Error.Validation("Session.RefreshTokenInvalido", "O refresh token é inválido.");

    public static readonly Error RefreshTokenExpirado =
        Error.Unauthorized("Session.RefreshTokenExpirado", "O refresh token expirou. Faça login novamente.");

    public static readonly Error SessaoRevogada =
        Error.Unauthorized("Session.Revogada", "Esta sessão foi revogada.");

    public static readonly Error SessaoExpirada =
        Error.Unauthorized("Session.Expirada", "Esta sessão expirou. Faça login novamente.");

    public static readonly Error SessaoNaoPertenceAoUsuario =
        Error.Forbidden("Session.NaoPertenceAoUsuario", "Esta sessão não pertence ao usuário.");

    public static readonly Error LimiteSessoesAtingido =
        Error.Validation("Session.LimiteSessoes", "Você atingiu o limite máximo de sessões ativas. Encerre uma sessão existente.");

    public static readonly Error DispositivoNaoReconhecido =
        Error.Validation("Session.DispositivoNaoReconhecido", "Dispositivo não reconhecido. Verifique seu email para autorizar este dispositivo.");
}
