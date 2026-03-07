namespace Catalog.Domain.Services;

/// <summary>
/// Serviço para armazenamento de imagens de produtos.
/// </summary>
public interface IImageStorageService
{
    /// <summary>
    /// Faz o upload de uma imagem e retorna a URL.
    /// </summary>
    /// <param name="fileName">Nome do arquivo.</param>
    /// <param name="stream">Stream do conteúdo do arquivo.</param>
    /// <param name="contentType">Tipo de conteúdo (MIME type).</param>
    /// <param name="cancellationToken">Token de cancelamento.</param>
    /// <returns>URL pública da imagem armazenada.</returns>
    Task<string> UploadImageAsync(string fileName, Stream stream, string contentType, CancellationToken cancellationToken = default);

    /// <summary>
    /// Remove uma imagem do armazenamento.
    /// </summary>
    /// <param name="imageUrl">URL da imagem a ser removida.</param>
    /// <param name="cancellationToken">Token de cancelamento.</param>
    Task DeleteImageAsync(string imageUrl, CancellationToken cancellationToken = default);
}
