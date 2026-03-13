using FluentValidation;

namespace Catalog.Application.Features.Review.Update;

public class UpdateReviewValidator : AbstractValidator<UpdateReviewInput>
{
    public UpdateReviewValidator()
    {
        RuleFor(x => x.ReviewId)
            .NotEmpty().WithMessage("O ID da avaliação é obrigatório.");

        RuleFor(x => x.Rating)
            .InclusiveBetween(1, 5).WithMessage("A nota deve estar entre 1 e 5.");

        RuleFor(x => x.Title)
            .MaximumLength(100).WithMessage("O título não pode ultrapassar 100 caracteres.")
            .When(x => !string.IsNullOrWhiteSpace(x.Title));

        RuleFor(x => x.Comment)
            .MaximumLength(500).WithMessage("O comentário não pode ultrapassar 500 caracteres.")
            .When(x => !string.IsNullOrWhiteSpace(x.Comment));
    }
}
