using Shared.Application.UseCases;

namespace Catalog.Application.Features.Review.Delete;

public interface IDeleteReviewUseCase : IUseCase<DeleteReviewInput, bool>
{
}
