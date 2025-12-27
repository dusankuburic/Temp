namespace Temp.Services.Abstractions;

public interface IStorageService
{
    Task<string> UploadAsync(Stream fileStream, string fileName, string contentType, CancellationToken cancellationToken = default);
    Task<Stream> DownloadAsync(string fileName, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(string fileName, CancellationToken cancellationToken = default);
    Task<bool> ExistsAsync(string fileName, CancellationToken cancellationToken = default);
    Task<string> GetDownloadUrlAsync(string fileName, TimeSpan expiresIn, CancellationToken cancellationToken = default);
    Task<IEnumerable<string>> ListFilesAsync(string? prefix = null, CancellationToken cancellationToken = default);
}