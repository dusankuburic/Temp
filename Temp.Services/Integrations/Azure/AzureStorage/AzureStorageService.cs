using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Azure.Storage.Sas;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Temp.Services.Abstractions;
using Temp.Services.Integrations.Azure.AzureStorage.Models;

namespace Temp.Services.Integrations.Azure.AzureStorage;

public class AzureStorageService : IAzureStorageService, IStorageService
{
    private readonly BlobServiceClient _blobServiceClient;
    private readonly AzureStorageOptions _options;
    private BlobContainerClient? _containerClient;

    private const string ImagesFolderPrefix = "images";
    private const string DocumentsFolderPrefix = "documents";

    public AzureStorageService(BlobServiceClient blobServiceClient, IOptions<AzureStorageOptions> options) {
        _blobServiceClient = blobServiceClient;
        _options = options.Value;
    }

    public static string UniqueFileName(string name) =>
        $"{DateTime.UtcNow:yyyyMMddHHmmssfff}_{SanitizeFileName(name)}";

    private static string SanitizeFileName(string fileName) =>
        string.Join("_", fileName.Split(Path.GetInvalidFileNameChars()));

    private static string GetFolderPrefix(FileType fileType) => fileType switch {
        FileType.Image => ImagesFolderPrefix,
        FileType.Document => DocumentsFolderPrefix,
        _ => "other"
    };

    private static string BuildFolderPath(FileType fileType, DateTime createdAt) =>
        $"{GetFolderPrefix(fileType)}/{createdAt:yyyy}/{createdAt:MM}/{createdAt:dd}";

    private static string BuildBlobPath(FileType fileType, DateTime createdAt, string fileName) =>
        $"{BuildFolderPath(fileType, createdAt)}/{UniqueFileName(fileName)}";

    private async Task<BlobContainerClient> GetContainerClientAsync(CancellationToken ct = default) {
        if (_containerClient != null)
            return _containerClient;

        _containerClient = _blobServiceClient.GetBlobContainerClient(_options.ContainerName);
        await _containerClient.CreateIfNotExistsAsync(cancellationToken: ct);

        return _containerClient;
    }

    private BlobResponseDto? ValidateFile(IFormFile file, FileType fileType) {
        if (file.Length == 0)
            return BlobResponseDto.Failure("File is empty", "FILE_EMPTY");

        if (file.Length > _options.MaxFileSizeBytes)
            return BlobResponseDto.Failure(
                $"File size {file.Length} exceeds maximum {_options.MaxFileSizeBytes} bytes",
                "FILE_TOO_LARGE");

        var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
        var allowedExtensions = fileType switch {
            FileType.Image => _options.AllowedImageExtensions,
            FileType.Document => _options.AllowedDocumentExtensions,
            _ => [.. _options.AllowedImageExtensions, .. _options.AllowedDocumentExtensions]
        };

        if (!allowedExtensions.Contains(extension))
            return BlobResponseDto.Failure(
                $"Extension '{extension}' not allowed. Allowed: {string.Join(", ", allowedExtensions)}",
                "INVALID_EXTENSION");

        return null;
    }



    public async Task<BlobResponseDto> UploadAsync(IFormFile blob, CancellationToken ct = default) {
        var container = await GetContainerClientAsync(ct);
        var fileName = UniqueFileName(blob.FileName);
        var client = container.GetBlobClient(fileName);

        await using var data = blob.OpenReadStream();
        await client.UploadAsync(data, new BlobHttpHeaders { ContentType = blob.ContentType }, cancellationToken: ct);

        return BlobResponseDto.Success(
            new BlobDto {
                Uri = client.Uri.AbsoluteUri,
                Name = client.Name,
                ContentType = blob.ContentType,
                Size = blob.Length
            },
            $"File {fileName} uploaded successfully");
    }

    public async Task<BlobResponseDto> UploadAsync(BlobUploadRequest request, CancellationToken ct = default) {
        var validationError = ValidateFile(request.File, request.FileType);
        if (validationError != null)
            return validationError;

        var container = await GetContainerClientAsync(ct);
        var createdAt = request.CreatedAt ?? DateTime.UtcNow;

        var blobPath = request.CustomFolder != null
            ? $"{request.CustomFolder}/{UniqueFileName(request.File.FileName)}"
            : BuildBlobPath(request.FileType, createdAt, request.File.FileName);

        var client = container.GetBlobClient(blobPath);

        await using var data = request.File.OpenReadStream();
        await client.UploadAsync(data, new BlobHttpHeaders { ContentType = request.File.ContentType }, cancellationToken: ct);

        var folderPath = blobPath.Contains('/') ? blobPath[..blobPath.LastIndexOf('/')] : string.Empty;

        return BlobResponseDto.Success(
            new BlobDto {
                Uri = client.Uri.AbsoluteUri,
                Name = client.Name,
                FolderPath = folderPath,
                FileType = request.FileType,
                CreatedAt = createdAt,
                ContentType = request.File.ContentType,
                Size = request.File.Length
            },
            $"File {blobPath} uploaded successfully");
    }

    public Task<BlobResponseDto> UploadImageAsync(IFormFile image, DateTime? createdAt = null, CancellationToken ct = default) =>
        UploadAsync(new BlobUploadRequest {
            File = image,
            FileType = FileType.Image,
            CreatedAt = createdAt
        }, ct);

    public Task<BlobResponseDto> UploadDocumentAsync(IFormFile document, DateTime? createdAt = null, CancellationToken ct = default) =>
        UploadAsync(new BlobUploadRequest {
            File = document,
            FileType = FileType.Document,
            CreatedAt = createdAt
        }, ct);

    public async Task<List<BlobDto>> ListAsync(CancellationToken ct = default) {
        var container = await GetContainerClientAsync(ct);
        var files = new List<BlobDto>();

        await foreach (var file in container.GetBlobsAsync(cancellationToken: ct)) {
            var fullUri = $"{container.Uri}/{file.Name}";
            var folderPath = file.Name.Contains('/') ? file.Name[..file.Name.LastIndexOf('/')] : string.Empty;

            files.Add(new BlobDto {
                Uri = fullUri,
                Name = file.Name,
                ContentType = file.Properties.ContentType,
                FolderPath = folderPath,
                CreatedAt = file.Properties.CreatedOn?.UtcDateTime,
                Size = file.Properties.ContentLength
            });
        }

        return files;
    }

    public async Task<List<BlobDto>> ListAsync(FileType fileType, DateTime? fromDate = null, DateTime? toDate = null, CancellationToken ct = default) {
        var container = await GetContainerClientAsync(ct);
        var files = new List<BlobDto>();
        var prefix = GetFolderPrefix(fileType);

        await foreach (var file in container.GetBlobsAsync(prefix: prefix, cancellationToken: ct)) {
            var createdAt = file.Properties.CreatedOn?.UtcDateTime;

            if (fromDate.HasValue && createdAt < fromDate.Value) continue;
            if (toDate.HasValue && createdAt > toDate.Value) continue;

            var fullUri = $"{container.Uri}/{file.Name}";
            var folderPath = file.Name.Contains('/') ? file.Name[..file.Name.LastIndexOf('/')] : string.Empty;

            files.Add(new BlobDto {
                Uri = fullUri,
                Name = file.Name,
                ContentType = file.Properties.ContentType,
                FolderPath = folderPath,
                FileType = fileType,
                CreatedAt = createdAt,
                Size = file.Properties.ContentLength
            });
        }

        return files;
    }

    public async Task<BlobDto?> DownloadAsync(string blobPath, CancellationToken ct = default) {
        var container = await GetContainerClientAsync(ct);
        var client = container.GetBlobClient(blobPath);

        if (!await client.ExistsAsync(ct))
            return null;

        var stream = await client.OpenReadAsync(cancellationToken: ct);
        var properties = await client.GetPropertiesAsync(cancellationToken: ct);

        return new BlobDto {
            Content = stream,
            Name = blobPath,
            ContentType = properties.Value.ContentType,
            Size = properties.Value.ContentLength,
            CreatedAt = properties.Value.CreatedOn.UtcDateTime
        };
    }

    public async Task<BlobResponseDto> DeleteAsync(string blobPath, CancellationToken ct = default) {
        var container = await GetContainerClientAsync(ct);
        var client = container.GetBlobClient(blobPath);

        var deleted = await client.DeleteIfExistsAsync(cancellationToken: ct);

        return deleted
            ? BlobResponseDto.Success(new BlobDto { Name = blobPath }, $"File {blobPath} deleted successfully")
            : BlobResponseDto.Failure($"File {blobPath} not found", "FILE_NOT_FOUND");
    }

    public async Task<bool> ExistsAsync(string blobPath, CancellationToken ct = default) {
        var container = await GetContainerClientAsync(ct);
        var client = container.GetBlobClient(blobPath);
        return await client.ExistsAsync(ct);
    }

    public async Task<string> GetDownloadUrlAsync(string blobPath, TimeSpan expiresIn, CancellationToken ct = default) {
        var container = await GetContainerClientAsync(ct);
        var client = container.GetBlobClient(blobPath);

        if (!await client.ExistsAsync(ct))
            throw new FileNotFoundException($"File '{blobPath}' not found in storage");

        var sasBuilder = new BlobSasBuilder {
            BlobContainerName = _options.ContainerName,
            BlobName = blobPath,
            Resource = "b",
            StartsOn = DateTimeOffset.UtcNow,
            ExpiresOn = DateTimeOffset.UtcNow.Add(expiresIn)
        };
        sasBuilder.SetPermissions(BlobSasPermissions.Read);

        return client.GenerateSasUri(sasBuilder).ToString();
    }

    public async Task<string> GenerateSasTokenForUploadAsync(string filename, FileType fileType, CancellationToken ct = default)
    {
        var container = await GetContainerClientAsync(ct);
        
        var extension = Path.GetExtension(filename).ToLowerInvariant();
        var allowedExtensions = fileType switch {
            FileType.Image => _options.AllowedImageExtensions,
            FileType.Document => _options.AllowedDocumentExtensions,
            _ => [.. _options.AllowedImageExtensions, .. _options.AllowedDocumentExtensions]
        };

        if (!allowedExtensions.Contains(extension))
            throw new InvalidOperationException($"File extension '{extension}' not allowed for {fileType}.");

        var blobClient = container.GetBlobClient(filename);

        var sasBuilder = new BlobSasBuilder
        {
            BlobContainerName = _options.ContainerName,
            BlobName = filename,
            Resource = "b",
            StartsOn = DateTimeOffset.UtcNow.AddMinutes(-1),
            ExpiresOn = DateTimeOffset.UtcNow.AddMinutes(5)
        };

        sasBuilder.SetPermissions(BlobSasPermissions.Write | BlobSasPermissions.Create);

        return blobClient.GenerateSasUri(sasBuilder).ToString();
    }


    async Task<string> IStorageService.UploadAsync(Stream fileStream, string fileName, string contentType, CancellationToken ct) {
        var container = await GetContainerClientAsync(ct);
        var uniqueFileName = UniqueFileName(fileName);
        var client = container.GetBlobClient(uniqueFileName);

        await client.UploadAsync(fileStream, new BlobHttpHeaders { ContentType = contentType }, cancellationToken: ct);
        return uniqueFileName;
    }

    async Task<Stream> IStorageService.DownloadAsync(string fileName, CancellationToken ct) {
        var container = await GetContainerClientAsync(ct);
        var client = container.GetBlobClient(fileName);

        if (!await client.ExistsAsync(ct))
            throw new FileNotFoundException($"File '{fileName}' not found in storage");

        return await client.OpenReadAsync(cancellationToken: ct);
    }

    async Task<bool> IStorageService.DeleteAsync(string fileName, CancellationToken ct) {
        var container = await GetContainerClientAsync(ct);
        var client = container.GetBlobClient(fileName);
        return await client.DeleteIfExistsAsync(cancellationToken: ct);
    }

    async Task<bool> IStorageService.ExistsAsync(string fileName, CancellationToken ct) {
        var container = await GetContainerClientAsync(ct);
        var client = container.GetBlobClient(fileName);
        return await client.ExistsAsync(ct);
    }

    async Task<string> IStorageService.GetDownloadUrlAsync(string fileName, TimeSpan expiresIn, CancellationToken ct) =>
        await GetDownloadUrlAsync(fileName, expiresIn, ct);

    async Task<IEnumerable<string>> IStorageService.ListFilesAsync(string? prefix, CancellationToken ct) {
        var container = await GetContainerClientAsync(ct);
        var files = new List<string>();

        await foreach (var file in container.GetBlobsAsync(prefix: prefix, cancellationToken: ct))
            files.Add(file.Name);

        return files;
    }

    
}
