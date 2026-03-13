namespace Catalog.Application.Features.Review.Create;

public record CreateReviewInput(
    Guid ProductId,
    int Rating,
    string? Title,
    string? Comment);
