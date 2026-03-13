namespace Catalog.Application.Features.Review.Update;

public record UpdateReviewInput(
    Guid ReviewId,
    int Rating,
    string? Title,
    string? Comment);
