using BuildingBlocks.Application.Models;

namespace Users.Domain.Errors;

/// <summary>
/// Erros relacionados à autenticação.
/// </summary>
public static class AuthErrors
{
    public static readonly Error CredenciaisInvalidas =
        Error.Unauthorized("Auth.CredenciaisInvalidas", "Email ou senha incorretos.");

    public static readonly Error UsuarioNaoEncontrado =
        Error.NotFound("Auth.UsuarioNaoEncontrado", "Usuário não encontrado.");

    public static readonly Error EmailJaCadastrado =
        Error.Conflict("Auth.EmailJaCadastrado", "Este email já está cadastrado.");

    public static readonly Error EmailNaoConfirmado =
        Error.Unauthorized("Auth.EmailNaoConfirmado", "Por favor, confirme seu email antes de fazer login.");

    public static readonly Error ContaBloqueada =
        Error.Unauthorized("Auth.ContaBloqueada", "Sua conta foi bloqueada temporariamente. Tente novamente mais tarde.");

    public static readonly Error ContaDesativada =
        Error.Unauthorized("Auth.ContaDesativada", "Sua conta foi desativada. Entre em contato com o suporte.");

    public static readonly Error SenhaFraca =
        Error.Validation("Auth.SenhaFraca", "A senha deve ter no mínimo 8 caracteres, incluindo letras maiúsculas, minúsculas, números e caracteres especiais.");

    public static readonly Error SenhaAtualIncorreta =
        Error.Validation("Auth.SenhaAtualIncorreta", "A senha atual está incorreta.");

    public static readonly Error NovaSenhaIgualAnterior =
        Error.Validation("Auth.NovaSenhaIgualAnterior", "A nova senha deve ser diferente da senha atual.");

    public static readonly Error TokenInvalido =
        Error.Validation("Auth.TokenInvalido", "Token inválido ou expirado.");

    public static readonly Error TokenExpirado =
        Error.Unauthorized("Auth.TokenExpirado", "O token expirou. Solicite um novo.");

    public static readonly Error MuitasTentativasLogin =
        Error.Unauthorized("Auth.MuitasTentativas", "Muitas tentativas de login. Aguarde alguns minutos antes de tentar novamente.");

    public static readonly Error EmailObrigatorio =
        Error.Validation("Auth.EmailObrigatorio", "O email é obrigatório.");

    public static readonly Error EmailInvalido =
        Error.Validation("Auth.EmailInvalido", "O email informado é inválido.");

    public static readonly Error SenhaObrigatoria =
        Error.Validation("Auth.SenhaObrigatoria", "A senha é obrigatória.");

    public static readonly Error ConfirmacaoSenhaDiferente =
        Error.Validation("Auth.ConfirmacaoSenhaDiferente", "A confirmação de senha não confere.");

    public static readonly Error ProvedorNaoSuportado =
        Error.Validation("Auth.ProvedorNaoSuportado", "Provedor de autenticação não suportado.");

    public static readonly Error EmailAlreadyConfirmed =
        Error.Conflict("Auth.EmailAlreadyConfirmed", "Este e-mail já foi confirmado. Faça login para continuar.");

    public static readonly Error AcessoNaoAutorizado =
        Error.Forbidden("Auth.AcessoNaoAutorizado", "Acesso negado. Você não tem permissão para acessar este recurso.");
}
