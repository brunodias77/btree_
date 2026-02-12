using BuildingBlocks.Application.Models;

namespace Users.Domain.Errors;

/// <summary>
/// Erros relacionados ao perfil do usuário.
/// </summary>
public static class ProfileErrors
{
    public static Error NaoEncontrado(Guid profileId) =>
        Error.NotFound("Profile.NotFound", $"Perfil com ID '{profileId}' não foi encontrado.");

    public static Error NaoEncontradoPorUsuario(Guid userId) =>
        Error.NotFound("Profile.NotFound", $"Perfil do usuário com ID '{userId}' não foi encontrado.");

    public static readonly Error JaExiste =
        Error.Conflict("Profile.Conflict", "Já existe um perfil para este usuário.");

    public static readonly Error CpfInvalido =
        Error.Validation("Profile.CpfInvalido", "O CPF informado é inválido.");

    public static readonly Error CpfJaCadastrado =
        Error.Conflict("Profile.CpfConflict", "Este CPF já está cadastrado para outro usuário.");

    public static readonly Error DataNascimentoInvalida =
        Error.Validation("Profile.DataNascimentoInvalida", "A data de nascimento informada é inválida.");

    public static readonly Error DataNascimentoFutura =
        Error.Validation("Profile.DataNascimentoFutura", "A data de nascimento não pode ser uma data futura.");

    public static readonly Error NomeObrigatorio =
        Error.Validation("Profile.NomeObrigatorio", "O nome é obrigatório.");

    public static readonly Error SobrenomeObrigatorio =
        Error.Validation("Profile.SobrenomeObrigatorio", "O sobrenome é obrigatório.");

    public static readonly Error AvatarUrlInvalida =
        Error.Validation("Profile.AvatarUrlInvalida", "A URL do avatar é inválida.");

    public static readonly Error TelefoneInvalido =
        Error.Validation("Profile.TelefoneInvalido", "O número de telefone é inválido.");

    public static readonly Error NomeInvalido =
        Error.Validation("Profile.NomeInvalido", "O nome é inválido.");

    public static readonly Error AcessoNaoAutorizado =
        Error.Failure("Profile.Forbidden", "Você não tem permissão para alterar este perfil."); // Using Failure with Forbidden code to map to 403
}
