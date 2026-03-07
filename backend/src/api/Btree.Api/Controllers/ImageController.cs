using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shared.Application.Abstractions;
using Shared.Web.Controllers;
using Shared.Web.Models;

namespace Btree.Api.Controllers;

[ApiController]
[Route("api/images")]
public class ImageController : ApiControllerBase
{
    private readonly IFileStorageService _fileStorageService;

    public ImageController(IFileStorageService fileStorageService)
    {
        _fileStorageService = fileStorageService;
    }

    /// <summary>
    /// Faz upload de uma imagem organizada por entidade.
    /// </summary>
    /// <param name="file">Arquivo de imagem (jpg, png, webp, svg, gif).</param>
    /// <param name="entity">Nome da entidade: brands, categories, products, etc.</param>
    [HttpPost("upload")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(ApiResponse<ImageUploadResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
    [RequestSizeLimit(5 * 1024 * 1024)] // 5 MB
    public async Task<IActionResult> Upload(IFormFile file, [FromQuery] string entity, CancellationToken cancellationToken)
    {
        // Validação do arquivo
        if (file is null || file.Length == 0)
        {
            return BadRequest(ApiErrorResponse.BadRequest("Image.NoFile", "Nenhum arquivo enviado."));
        }

        if (file.Length > _fileStorageService.MaxFileSizeBytes)
        {
            return BadRequest(ApiErrorResponse.BadRequest("Image.TooLarge", $"O arquivo excede o tamanho máximo de {_fileStorageService.MaxFileSizeBytes / (1024 * 1024)} MB."));
        }

        // Validação da entidade
        var allowedEntities = new[] { "brands", "categories", "products", "banners", "users" };
        var sanitizedEntity = entity?.Trim().ToLowerInvariant() ?? string.Empty;
        if (string.IsNullOrWhiteSpace(sanitizedEntity) || !allowedEntities.Contains(sanitizedEntity))
        {
            return BadRequest(ApiErrorResponse.BadRequest("Image.InvalidEntity", $"Entidade inválida. Valores permitidos: {string.Join(", ", allowedEntities)}."));
        }

        // Validação da extensão do arquivo
        var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
        if (!_fileStorageService.AllowedExtensions.Contains(extension))
        {
            return BadRequest(ApiErrorResponse.BadRequest("Image.InvalidExtension", $"Extensão '{extension}' não permitida. Extensões válidas: {string.Join(", ", _fileStorageService.AllowedExtensions)}."));
        }

        try
        {
            await using var stream = file.OpenReadStream();
            var url = await _fileStorageService.UploadAsync(stream, file.FileName, sanitizedEntity, cancellationToken);

            return Ok(ApiResponse<ImageUploadResponse>.Ok(new ImageUploadResponse(url)));
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ApiErrorResponse.BadRequest("Image.UploadFailed", ex.Message));
        }
    }

    /// <summary>
    /// Remove uma imagem pelo caminho relativo.
    /// </summary>
    /// <param name="url">URL relativa da imagem (ex: /uploads/brands/guid_logo.png).</param>
    [HttpDelete]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete([FromQuery] string url, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(url))
        {
            return BadRequest(ApiErrorResponse.BadRequest("Image.UrlRequired", "A URL da imagem é obrigatória."));
        }

        var deleted = await _fileStorageService.DeleteAsync(url, cancellationToken);
        if (!deleted)
        {
            return NotFound(ApiErrorResponse.NotFound("Imagem não encontrada."));
        }

        return Ok(ApiResponse.Ok("Imagem removida com sucesso."));
    }
}

public record ImageUploadResponse(string Url);
