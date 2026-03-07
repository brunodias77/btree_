using Shared.Application.Models;

namespace Shared.Application.UseCases;

public interface IUseCase<in TInput, TOutput>
{
    Task<Result<TOutput>> ExecuteAsync(TInput input, CancellationToken cancellationToken = default);
}