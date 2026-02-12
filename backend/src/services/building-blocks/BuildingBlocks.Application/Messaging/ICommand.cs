using BuildingBlocks.Application.Models;
using MediatR;

namespace BuildingBlocks.Application.Messaging;

/// <summary>
/// Interface marcadora para comandos (operações de escrita).
/// Comandos modificam estado e retornam Result.
/// </summary>
public interface ICommand : IRequest<Result>
{
}

/// <summary>
/// Interface marcadora para comandos que retornam um valor tipado.
/// </summary>
/// <typeparam name="TResponse">Tipo do valor de retorno.</typeparam>
public interface ICommand<TResponse> : IRequest<Result<TResponse>>
{
}