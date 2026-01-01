using Microsoft.AspNetCore.Http;
using Temp.Services.Integrations.Azure.AzureStorage.Models;

namespace Temp.Services.Integrations.Azure.AzureStorage;

public interface IAzureStorageService
{
    Task<List<BlobDto>> ListAsync(CancellationToken ct = default);
    Task<List<BlobDto>> ListAsync(FileType fileType, DateTime? fromDate = null, DateTime? toDate = null, CancellationToken ct = default);

    Task<BlobResponseDto> UploadAsync(IFormFile blob, CancellationToken ct = default);
    Task<BlobResponseDto> UploadAsync(BlobUploadRequest request, CancellationToken ct = default);
    Task<BlobResponseDto> UploadImageAsync(IFormFile image, DateTime? createdAt = null, CancellationToken ct = default);
    Task<BlobResponseDto> UploadDocumentAsync(IFormFile document, DateTime? createdAt = null, CancellationToken ct = default);

    Task<BlobDto?> DownloadAsync(string blobPath, CancellationToken ct = default);
    Task<BlobResponseDto> DeleteAsync(string blobPath, CancellationToken ct = default);
    Task<bool> ExistsAsync(string blobPath, CancellationToken ct = default);
    Task<string> GetDownloadUrlAsync(string blobPath, TimeSpan expiresIn, CancellationToken ct = default);
    Task<string> GenerateSasTokenForUploadAsync(string filename, FileType fileType, CancellationToken ct = default);
}