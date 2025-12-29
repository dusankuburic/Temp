using Microsoft.AspNetCore.Http;

namespace Temp.Services.Integrations.Azure.AzureStorage.Models;

public class BlobUploadRequest
{
    public required IFormFile File { get; init; }
    public required FileType FileType { get; init; }
    public DateTime? CreatedAt { get; init; }
    public string? CustomFolder { get; init; }
}
