using BuildingBlocks.Application.Models;

namespace Users.Domain.Errors;

/// <summary>
/// Erros relacionados a endereços.
/// </summary>
public static class AddressErrors
{
    public static Error NaoEncontrado(Guid addressId) =>
        Error.NotFound("Address.NaoEncontrado", $"Endereço com ID '{addressId}' não foi encontrado.");

    public static readonly Error EnderecoNaoEncontrado =
        Error.NotFound("Address.NaoEncontrado", "Endereço não foi encontrado.");

    public static readonly Error RuaObrigatoria =
        Error.Validation("Address.RuaObrigatoria", "A rua é obrigatória.");

    public static readonly Error CidadeObrigatoria =
        Error.Validation("Address.CidadeObrigatoria", "A cidade é obrigatória.");

    public static readonly Error EstadoObrigatorio =
        Error.Validation("Address.EstadoObrigatorio", "O estado é obrigatório.");

    public static readonly Error EstadoInvalido =
        Error.Validation("Address.EstadoInvalido", "O estado informado é inválido. Use a sigla de 2 letras (ex: SP, RJ).");

    public static readonly Error CepObrigatorio =
        Error.Validation("Address.CepObrigatorio", "O CEP é obrigatório.");

    public static readonly Error CepInvalido =
        Error.Validation("Address.CepInvalido", "O CEP informado é inválido.");

    public static readonly Error LimiteEnderecos =
        Error.Validation("Address.LimiteEnderecos", "Você atingiu o limite máximo de endereços cadastrados.");

    public static readonly Error EnderecoNaoPertenceAoUsuario =
        Error.Forbidden("Address.NaoPertenceAoUsuario", "Este endereço não pertence ao usuário.");

    public static readonly Error NaoPodeDeletarEnderecoPadrao =
        Error.Validation("Address.NaoPodeDeletarPadrao", "Não é possível excluir o endereço padrão. Defina outro endereço como padrão primeiro.");

    public static readonly Error NaoPermitidoRemoverPadrao =
        Error.Validation("Address.NaoPermitidoRemoverPadrao", "Não é possível remover o status de padrão do único endereço cadastrado.");
}
