using Catalog.Domain.Services;
using Shared.Application.Abstractions;

namespace Catalog.Infrastructure.Services;

/// <summary>
/// Implementação local de armazenamento de imagens da Catalog,
/// reaproveitando o serviço compartilhado IFileStorageService.
/// </summary>
public class LocalImageStorageService : IImageStorageService
{
    private readonly IFileStorageService _fileStorageService;

    public LocalImageStorageService(IFileStorageService fileStorageService)
    {
        _fileStorageService = fileStorageService;
    }

    public async Task<string> UploadImageAsync(string fileName, Stream stream, string contentType, CancellationToken cancellationToken = default)
    {
        // Delega o upload para a implementação genérica, especificando a pasta da entidade (ex: products).
        // Isso vai colocar os arquivos em wwwroot/uploads/products
        var relativeUrl = await _fileStorageService.UploadAsync(
            stream, 
            fileName, 
            "products", 
            generateUniqueName: false,
            cancellationToken);

        return relativeUrl;
    }

    public async Task DeleteImageAsync(string imageUrl, CancellationToken cancellationToken = default)
    {
        // Delega a deleção para a abstração de file storage (que limpa strings e remove fisicamente do disco)
        await _fileStorageService.DeleteAsync(imageUrl, cancellationToken);
    }
}
