using BuildingBlocks.Application.Models;
using MediatR;

namespace BuildingBlocks.Application.Messaging;

/// <summary>
/// Interface marcadora para queries (operações de leitura).
/// Queries não modificam estado e retornam dados.
/// </summary>
/// <typeparam name="TResponse">Tipo do valor de retorno.</typeparam>
public interface IQuery<TResponse> : IRequest<Result<TResponse>>
{
}