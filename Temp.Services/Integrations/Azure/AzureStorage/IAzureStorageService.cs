using Microsoft.AspNetCore.Http;
using Temp.Services.Integrations.Azure.AzureStorage.Models;

namespace Temp.Services.Integrations.Azure.AzureStorage;

public interface IAzureStorageService
{
    Task<List<BlobDto>> ListAsync();
    Task<BlobResponseDto> UploadAsync(IFormFile blob);
    Task<BlobDto> DownloadAsync(string blobFilename);
    Task<BlobResponseDto> DeleteAsync(string blobFilename);
}