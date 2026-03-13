using Shared.Application.UseCases;

namespace Catalog.Application.Features.Review.Create;

public interface ICreateReviewUseCase : IUseCase<CreateReviewInput, Guid>
{
}
