using Shared.Application.Abstractions;

namespace Shared.Infrastructure.Services;

/// <summary>
/// Implementação de armazenamento de arquivos no disco local (wwwroot/uploads).
/// </summary>
public class LocalFileStorageService : IFileStorageService
{
    private readonly string _basePath;

    public string[] AllowedExtensions => new[] { ".jpg", ".jpeg", ".png", ".webp", ".svg", ".gif" };
    public long MaxFileSizeBytes => 5 * 1024 * 1024; // 5 MB

    public LocalFileStorageService(string basePath)
    {
        _basePath = basePath;
        Directory.CreateDirectory(_basePath);
    }

    public async Task<string> UploadAsync(Stream fileStream, string fileName, string entity, bool generateUniqueName = true, CancellationToken cancellationToken = default)
    {
        // Sanitizar nome da entidade (lowercase, sem caracteres especiais)
        var sanitizedEntity = entity.Trim().ToLowerInvariant();

        // Validar extensão
        var extension = Path.GetExtension(fileName).ToLowerInvariant();
        if (!AllowedExtensions.Contains(extension))
        {
            throw new InvalidOperationException($"Extensão '{extension}' não é permitida. Extensões válidas: {string.Join(", ", AllowedExtensions)}");
        }

        // Gerar nome único ou usar o nome fornecido
        var finalFileName = generateUniqueName 
            ? $"{Guid.NewGuid()}{extension}" 
            : Path.GetFileName(fileName);

        // Criar diretório se não existir
        var entityDir = Path.Combine(_basePath, sanitizedEntity);
        Directory.CreateDirectory(entityDir);

        // Salvar arquivo
        var filePath = Path.Combine(entityDir, finalFileName);
        await using var outputStream = new FileStream(filePath, FileMode.Create, FileAccess.Write);
        await fileStream.CopyToAsync(outputStream, cancellationToken);

        // Retornar URL relativa
        return $"/uploads/{sanitizedEntity}/{finalFileName}";
    }

    public Task<bool> DeleteAsync(string relativeUrl, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(relativeUrl))
            return Task.FromResult(false);

        // Converter URL relativa para caminho absoluto
        // relativeUrl: /uploads/brands/guid_logo.png → basePath/../brands/guid_logo.png
        var relativePath = relativeUrl.TrimStart('/').Replace("uploads/", "");
        var filePath = Path.Combine(_basePath, relativePath);

        if (!File.Exists(filePath))
            return Task.FromResult(false);

        File.Delete(filePath);
        return Task.FromResult(true);
    }
}
