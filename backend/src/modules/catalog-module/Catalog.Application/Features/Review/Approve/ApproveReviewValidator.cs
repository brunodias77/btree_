using FluentValidation;

namespace Catalog.Application.Features.Review.Approve;

public class ApproveReviewValidator : AbstractValidator<ApproveReviewInput>
{
    public ApproveReviewValidator()
    {
        RuleFor(x => x.ReviewId)
            .NotEmpty().WithMessage("O ID da avaliação é obrigatório.");
    }
}
