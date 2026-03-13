using FluentValidation;

namespace Catalog.Application.Features.Review.Delete;

public class DeleteReviewValidator : AbstractValidator<DeleteReviewInput>
{
    public DeleteReviewValidator()
    {
        RuleFor(x => x.ReviewId)
            .NotEmpty().WithMessage("O ID da avaliação é obrigatório.");
    }
}
