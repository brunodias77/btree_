using Shared.Application.Models;

namespace Catalog.Application.Features.Categories.Get;

public record GetCategoriesInput : PagedRequest
{
    public string? SearchTerm { get; init; }
    public Guid? ParentId { get; init; }
    public bool? IsActive { get; init; }
}
