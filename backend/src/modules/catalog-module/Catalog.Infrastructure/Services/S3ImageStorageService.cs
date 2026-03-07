using Catalog.Domain.Services;

namespace Catalog.Infrastructure.Services;

public class S3ImageStorageService : IImageStorageService
{
    public Task<string> UploadImageAsync(string fileName, Stream stream, string contentType, CancellationToken cancellationToken = default)
    {
        // TODO: Implement S3 logic using AWS SDK
        throw new NotImplementedException("S3 Storage not implemented yet.");
    }

    public Task DeleteImageAsync(string imageUrl, CancellationToken cancellationToken = default)
    {
        // TODO: Implement S3 logic using AWS SDK
        throw new NotImplementedException("S3 Storage not implemented yet.");
    }
}
