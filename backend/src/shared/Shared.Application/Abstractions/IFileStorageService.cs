namespace Shared.Application.Abstractions;

/// <summary>
/// Abstração para armazenamento de arquivos (imagens, documentos, etc.).
/// </summary>
public interface IFileStorageService
{
    /// <summary>
    /// Extensões de arquivo permitidas.
    /// </summary>
    string[] AllowedExtensions { get; }

    /// <summary>
    /// Tamanho máximo do arquivo em bytes.
    /// </summary>
    long MaxFileSizeBytes { get; }

    /// <summary>
    /// Faz upload de um arquivo e retorna a URL relativa.
    /// </summary>
    /// <param name="fileStream">Stream do arquivo.</param>
    /// <param name="fileName">Nome original do arquivo.</param>
    /// <param name="entity">Nome da entidade (ex: "brands", "categories", "products").</param>
    /// <param name="cancellationToken">Token de cancelamento.</param>
    /// <returns>URL relativa do arquivo salvo (ex: /uploads/brands/guid_logo.png).</returns>
    Task<string> UploadAsync(Stream fileStream, string fileName, string entity, CancellationToken cancellationToken = default);

    /// <summary>
    /// Remove um arquivo pelo caminho relativo.
    /// </summary>
    /// <param name="relativeUrl">URL relativa do arquivo (ex: /uploads/brands/guid_logo.png).</param>
    /// <param name="cancellationToken">Token de cancelamento.</param>
    /// <returns>True se removido com sucesso.</returns>
    Task<bool> DeleteAsync(string relativeUrl, CancellationToken cancellationToken = default);
}
