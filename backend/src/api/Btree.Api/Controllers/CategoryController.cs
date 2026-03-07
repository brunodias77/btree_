using Shared.Web.Controllers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shared.Web.Models;
using Catalog.Application.Features.Categories.Create;
using Catalog.Application.Features.Categories.Update;
using Catalog.Application.Features.Categories.Delete;
using Catalog.Application.Features.Categories.Get;
using Shared.Application.Models;

namespace Btree.Api.Controllers;

[Route("api/categories")]
public class CategoryController : ApiControllerBase
{
    private readonly ICreateCategoryUseCase _createCategoryUseCase;
    private readonly IUpdateCategoryUseCase _updateCategoryUseCase;
    private readonly IDeleteCategoryUseCase _deleteCategoryUseCase;
    private readonly IGetCategoriesUseCase _getCategoriesUseCase;

    public CategoryController(
        ICreateCategoryUseCase createCategoryUseCase,
        IUpdateCategoryUseCase updateCategoryUseCase,
        IDeleteCategoryUseCase deleteCategoryUseCase,
        IGetCategoriesUseCase getCategoriesUseCase)
    {
        _createCategoryUseCase = createCategoryUseCase;
        _updateCategoryUseCase = updateCategoryUseCase;
        _deleteCategoryUseCase = deleteCategoryUseCase;
        _getCategoriesUseCase = getCategoriesUseCase;
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(ApiResponse<Guid>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> CreateCategory([FromBody] CreateCategoryInput request, CancellationToken cancellationToken)
    {
        var result = await _createCategoryUseCase.ExecuteAsync(request, cancellationToken);

        return HandleResult(result);
    }

    [HttpPut("{id:guid}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(ApiResponse<Guid>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> UpdateCategory([FromRoute] Guid id, [FromBody] UpdateCategoryRequest request, CancellationToken cancellationToken)
    {
        var input = new UpdateCategoryInput(
            id,
            request.Name,
            request.Slug,
            request.Description,
            request.ImageUrl,
            request.MetaTitle,
            request.MetaDescription,
            request.SortOrder);

        var result = await _updateCategoryUseCase.ExecuteAsync(input, cancellationToken);

        return HandleResult(result);
    }

    [HttpDelete("{id:guid}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(ApiResponse<Guid>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteCategory([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        var input = new DeleteCategoryInput(id);

        var result = await _deleteCategoryUseCase.ExecuteAsync(input, cancellationToken);

        return HandleResult(result);
    }

    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<PagedResult<CategoryOutput>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetCategories([FromQuery] GetCategoriesInput input, CancellationToken cancellationToken)
    {
        var result = await _getCategoriesUseCase.ExecuteAsync(input, cancellationToken);

        return HandleResult(result);
    }
}

public record UpdateCategoryRequest(
    string Name,
    string? Slug,
    string? Description,
    string? ImageUrl,
    string? MetaTitle,
    string? MetaDescription,
    int SortOrder);
