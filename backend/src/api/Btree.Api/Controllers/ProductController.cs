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
    private readonly Catalog.Application.Features.Review.Create.ICreateReviewUseCase _createReviewUseCase;
    private readonly Catalog.Application.Features.Review.Update.IUpdateReviewUseCase _updateReviewUseCase;
    private readonly Catalog.Application.Features.Review.Delete.IDeleteReviewUseCase _deleteReviewUseCase;

    public ProductController(
        IAddProductImageUseCase addProductImageUseCase,
        IRemoveProductImageUseCase removeProductImageUseCase,
        Catalog.Application.Features.Review.Create.ICreateReviewUseCase createReviewUseCase,
        Catalog.Application.Features.Review.Update.IUpdateReviewUseCase updateReviewUseCase,
        Catalog.Application.Features.Review.Delete.IDeleteReviewUseCase deleteReviewUseCase)
    {
        _addProductImageUseCase = addProductImageUseCase;
        _removeProductImageUseCase = removeProductImageUseCase;
        _createReviewUseCase = createReviewUseCase;
        _updateReviewUseCase = updateReviewUseCase;
        _deleteReviewUseCase = deleteReviewUseCase;
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

    /// <summary>
    /// Avalia um produto.
    /// </summary>
    /// <param name="productId">ID do produto.</param>
    /// <param name="request">Dados da avaliação.</param>
    [HttpPost("{productId:guid}/reviews")]
    [Authorize]
    [ProducesResponseType(typeof(ApiResponse<Guid>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> CreateReview(
        [FromRoute] Guid productId,
        [FromBody] CreateReviewRequest request,
        CancellationToken cancellationToken = default)
    {
        var input = new Catalog.Application.Features.Review.Create.CreateReviewInput(
            ProductId: productId,
            Rating: request.Rating,
            Title: request.Title,
            Comment: request.Comment);

        var result = await _createReviewUseCase.ExecuteAsync(input, cancellationToken);

        if (result.IsFailure)
        {
            if (result.Error.Code == "CATALOG.PRODUCT_NOT_FOUND")
            {
                return NotFound(ApiErrorResponse.NotFound(result.Error.Message));
            }
            if (result.Error.Code == "ProductReview.Conflict") // The code from ReviewErrors usually has the Aggregate name if it's Conflict
            {
                 return Conflict(ApiErrorResponse.Conflict(result.Error.Message));
            }

            return BadRequest(ApiErrorResponse.BadRequest(result.Error.Code, result.Error.Message));
        }

        return Created($"/api/products/{productId}/reviews/{result.Value}", ApiResponse<Guid>.Ok(result.Value));
    }

    /// <summary>
    /// Atualiza uma avaliação existente de um produto.
    /// </summary>
    /// <param name="productId">ID do produto.</param>
    /// <param name="reviewId">ID da avaliação.</param>
    /// <param name="request">Novos dados da avaliação.</param>
    [HttpPut("{productId:guid}/reviews/{reviewId:guid}")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateReview(
        [FromRoute] Guid productId,
        [FromRoute] Guid reviewId,
        [FromBody] UpdateReviewRequest request,
        CancellationToken cancellationToken = default)
    {
        var input = new Catalog.Application.Features.Review.Update.UpdateReviewInput(
            ReviewId: reviewId,
            Rating: request.Rating,
            Title: request.Title,
            Comment: request.Comment);

        var result = await _updateReviewUseCase.ExecuteAsync(input, cancellationToken);

        if (result.IsFailure)
        {
            if (result.Error.Code == "ProductReview.NotFound")
            {
                return NotFound(ApiErrorResponse.NotFound(result.Error.Message));
            }
            if (result.Error.Code == "Review.NotOwner" || result.Error.Code == "Unauthorized")
            {
                return StatusCode(StatusCodes.Status403Forbidden, ApiErrorResponse.Forbidden(result.Error.Message));
            }

            return BadRequest(ApiErrorResponse.BadRequest(result.Error.Code, result.Error.Message));
        }

        return NoContent();
    }

    /// <summary>
    /// Exclui uma avaliação existente de um produto (Soft Delete).
    /// Apenas o autor da avaliação ou um administrador podem executar esta ação.
    /// </summary>
    /// <param name="productId">ID do produto.</param>
    /// <param name="reviewId">ID da avaliação.</param>
    [HttpDelete("{productId:guid}/reviews/{reviewId:guid}")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteReview(
        [FromRoute] Guid productId,
        [FromRoute] Guid reviewId,
        CancellationToken cancellationToken = default)
    {
        var input = new Catalog.Application.Features.Review.Delete.DeleteReviewInput(reviewId);

        var result = await _deleteReviewUseCase.ExecuteAsync(input, cancellationToken);

        if (result.IsFailure)
        {
            if (result.Error.Code == "ProductReview.NotFound")
            {
                return NotFound(ApiErrorResponse.NotFound(result.Error.Message));
            }
            if (result.Error.Code == "Review.NotOwner" || result.Error.Code == "Unauthorized")
            {
                return StatusCode(StatusCodes.Status403Forbidden, ApiErrorResponse.Forbidden(result.Error.Message));
            }

            return BadRequest(ApiErrorResponse.BadRequest(result.Error.Code, result.Error.Message));
        }

        return NoContent();
    }
}

public record CreateReviewRequest(int Rating, string? Title, string? Comment);
public record UpdateReviewRequest(int Rating, string? Title, string? Comment);
