using Shared.Application.Models;

namespace Catalog.Domain.Errors;

/// <summary>
/// Erros relacionados a imagens de produtos.
/// </summary>
public static class ImageErrors
{
    public static Error MaxImagesExceeded =>
        Error.Failure("Image.MaxLimitExceeded", "O produto já possui o número máximo de imagens permitido (10).");

    public static Error NotFound =>
        Error.NotFound("CATALOG.IMAGE_NOT_FOUND", "A imagem especificada não pertence a este produto ou já foi removida.");

    public static Error DeletionFailed =>
        Error.Failure("CATALOG.IMAGE_DELETION_FAILED", "Falha ao remover o ficheiro físico do servidor de armazenamento.");

    public static Error UploadFailed =>
        Error.Failure("Image.UploadFailed", "Falha ao processar e armazenar a imagem no servidor. URL retornada vazia.");

    public static Error UploadError(string details) =>
        Error.Failure("Image.UploadError", $"Ocorreu um erro ao adicionar a imagem: {details}");
}
