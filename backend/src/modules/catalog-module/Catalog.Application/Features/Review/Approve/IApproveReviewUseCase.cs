using Shared.Application.UseCases;

namespace Catalog.Application.Features.Review.Approve;

public interface IApproveReviewUseCase : IUseCase<ApproveReviewInput, bool>
{
}
