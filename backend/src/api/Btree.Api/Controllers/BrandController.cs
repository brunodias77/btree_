using Catalog.Application.Features.Brands.Commands.CreateBrand;
using Catalog.Application.Features.Brands.Commands.DeleteBrand;
using Catalog.Application.Features.Brands.Commands.UpdateBrand;
using Catalog.Application.Features.Brands.Queries.GetBrands;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shared.Application.Models;
using Shared.Web.Controllers;
using Shared.Web.Models;

namespace Btree.Api.Controllers;

[ApiController]
[Route("api/brands")]
public class BrandController : ApiControllerBase
{
    private readonly ICreateBrandUseCase _createBrandUseCase;
    private readonly IUpdateBrandUseCase _updateBrandUseCase;
    private readonly IDeleteBrandUseCase _deleteBrandUseCase;
    private readonly IGetBrandsUseCase _getBrandsUseCase;

    public BrandController(
        ICreateBrandUseCase createBrandUseCase,
        IUpdateBrandUseCase updateBrandUseCase,
        IDeleteBrandUseCase deleteBrandUseCase,
        IGetBrandsUseCase getBrandsUseCase)
    {
        _createBrandUseCase = createBrandUseCase;
        _updateBrandUseCase = updateBrandUseCase;
        _deleteBrandUseCase = deleteBrandUseCase;
        _getBrandsUseCase = getBrandsUseCase;
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(ApiResponse<Guid>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateBrand([FromBody] CreateBrandInput input, CancellationToken cancellationToken)
    {
        var result = await _createBrandUseCase.ExecuteAsync(input, cancellationToken);

        if (result.IsSuccess)
        {
            // Opcional: usar CreatedAtAction quando tiver o GetById
            return StatusCode(StatusCodes.Status201Created, ApiResponse<Guid>.Ok(result.Value));
        }

        return HandleResult(result);
    }

    [HttpPut("{id:guid}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> UpdateBrand([FromRoute] Guid id, [FromBody] UpdateBrandRequest request, CancellationToken cancellationToken)
    {
        var input = new UpdateBrandInput(
            id,
            request.Name,
            request.Description,
            request.LogoUrl,
            request.IsActive);

        var result = await _updateBrandUseCase.ExecuteAsync(input, cancellationToken);

        return HandleResult(result);
    }

    [HttpDelete("{id:guid}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> DeleteBrand([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        var input = new DeleteBrandInput(id);

        var result = await _deleteBrandUseCase.ExecuteAsync(input, cancellationToken);

        return HandleResult(result);
    }

    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<PagedResult<BrandOutput>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetBrands([FromQuery] GetBrandsInput input, CancellationToken cancellationToken)
    {
        var result = await _getBrandsUseCase.ExecuteAsync(input, cancellationToken);

        return HandleResult(result);
    }
}

public record UpdateBrandRequest(
    string Name,
    string? Description,
    string? LogoUrl,
    bool IsActive);
