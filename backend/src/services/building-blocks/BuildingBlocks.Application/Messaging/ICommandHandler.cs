using BuildingBlocks.Application.Models;
using MediatR;

namespace BuildingBlocks.Application.Messaging;

/// <summary>
/// Interface para handlers de comandos sem retorno de valor.
/// </summary>
/// <typeparam name="TCommand">Tipo do comando.</typeparam>
public interface ICommandHandler<in TCommand> : IRequestHandler<TCommand, Result>
    where TCommand : ICommand
{
}

/// <summary>
/// Interface para handlers de comandos com retorno de valor tipado.
/// </summary>
/// <typeparam name="TCommand">Tipo do comando.</typeparam>
/// <typeparam name="TResponse">Tipo do valor de retorno.</typeparam>
public interface ICommandHandler<in TCommand, TResponse> : IRequestHandler<TCommand, Result<TResponse>>
    where TCommand : ICommand<TResponse>
{
}

/// <summary>
/// Interface para handlers de queries.
/// </summary>
/// <typeparam name="TQuery">Tipo da query.</typeparam>
/// <typeparam name="TResponse">Tipo do valor de retorno.</typeparam>
public interface IQueryHandler<in TQuery, TResponse> : IRequestHandler<TQuery, Result<TResponse>>
    where TQuery : IQuery<TResponse>
{
}
