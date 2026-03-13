using Catalog.Application.Features.Products.AddImage;
using Catalog.Application.Features.Products.RemoveImage;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shared.Web.Controllers;
using Shared.Web.Models;

namespace Btree.Api.Controllers;

[ApiController]
[Route("api/products")]
public class ProductController : ApiControllerBase
{
    private readonly IAddProductImageUseCase _addProductImageUseCase;
    private readonly IRemoveProductImageUseCase _removeProductImageUseCase;

    public ProductController(
        IAddProductImageUseCase addProductImageUseCase,
        IRemoveProductImageUseCase removeProductImageUseCase)
    {
        _addProductImageUseCase = addProductImageUseCase;
        _removeProductImageUseCase = removeProductImageUseCase;
    }

    /// <summary>
    /// Adiciona uma imagem a um produto do catálogo.
    /// </summary>
    /// <param name="productId">ID do produto.</param>
    /// <param name="file">Arquivo de imagem (jpg, png, webp).</param>
    /// <param name="isPrimary">Indica se a imagem será a principal (capa) do produto.</param>
    [HttpPost("{productId:guid}/images")]
    [Authorize(Roles = "Admin")]
    [Consumes("multipart/form-data")]
    [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> AddImage(
        [FromRoute] Guid productId,
        [FromForm] IFormFile file,
        [FromForm] bool isPrimary = false,
        CancellationToken cancellationToken = default)
    {
        if (file is null || file.Length == 0)
        {
            return BadRequest(ApiErrorResponse.BadRequest("Image.NoFile", "Nenhum arquivo enviado."));
        }

        await using var stream = file.OpenReadStream();

        var input = new AddProductImageInput(
            ProductId: productId,
            ImageStream: stream,
            FileName: file.FileName,
            ContentType: file.ContentType,
            IsPrimary: isPrimary);

        var result = await _addProductImageUseCase.ExecuteAsync(input, cancellationToken);

        if (result.IsFailure)
        {
            if (result.Error.Code == "CATALOG.PRODUCT_NOT_FOUND")
            {
                return NotFound(ApiErrorResponse.NotFound(result.Error.Message));
            }

            return BadRequest(ApiErrorResponse.BadRequest(result.Error.Code, result.Error.Message));
        }

        return Ok(ApiResponse<string>.Ok(result.Value));
    }

    /// <summary>
    /// Remove uma imagem de um produto do catálogo.
    /// </summary>
    /// <param name="productId">ID do produto.</param>
    /// <param name="imageId">ID da imagem.</param>
    [HttpDelete("{productId:guid}/images/{imageId:guid}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> RemoveImage(
        [FromRoute] Guid productId,
        [FromRoute] Guid imageId,
        CancellationToken cancellationToken = default)
    {
        var input = new Catalog.Application.Features.Products.RemoveImage.RemoveProductImageInput(productId, imageId);
        
        var result = await _removeProductImageUseCase.ExecuteAsync(input, cancellationToken);

        if (result.IsFailure)
        {
            if (result.Error.Code == "CATALOG.PRODUCT_NOT_FOUND" || result.Error.Code == "CATALOG.IMAGE_NOT_FOUND")
            {
                return NotFound(ApiErrorResponse.NotFound(result.Error.Message));
            }

            return BadRequest(ApiErrorResponse.BadRequest(result.Error.Code, result.Error.Message));
        }

        return NoContent();
    }
}
