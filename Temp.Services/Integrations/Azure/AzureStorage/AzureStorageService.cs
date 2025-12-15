using Azure.Storage.Blobs;
using Azure.Storage.Sas;
using Microsoft.AspNetCore.Http;
using Temp.Services.Abstractions;
using Temp.Services.Integrations.Azure.AzureStorage.Models;

namespace Temp.Services.Integrations.Azure.AzureStorage;


public class AzureStorageService : IAzureStorageService, IStorageService
{
    private readonly string _connectionString;
    private readonly string _metadataKeyTitle = "title";
    private readonly string _metadataKeyDescription = "description";

    public AzureStorageService(string connectionString) {
        _connectionString = connectionString;
    }

    private string _containerName = "files";
    public string ContainerName
    {
        get { return _connectionString; }
        set { _containerName = value; }
    }


    public static string UniqueFileName(string name) =>
        $"{DateTime.UtcNow.ToString("yyyyMMddHHmmssfff")}_{name}";


    private async Task<BlobContainerClient> GetStorageClient() {
        var cloudBlobContainer = new BlobServiceClient(_connectionString)
            .GetBlobContainerClient(_containerName);

        await cloudBlobContainer.CreateIfNotExistsAsync();

        return cloudBlobContainer;
    }

    public async Task<BlobResponseDto> UploadAsync(IFormFile blob) {
        var cloudBlobContainer = await GetStorageClient();
        var fileName = UniqueFileName(blob.FileName);
        var client = cloudBlobContainer.GetBlobClient(fileName);

        await using (Stream data = blob.OpenReadStream()) {
            await client.UploadAsync(data);
        }

        BlobResponseDto response = new() {
            Status = $"File {fileName} Uploaded Successfully",
            Error = false
        };

        response.Blob.Uri = client.Uri.AbsoluteUri;
        response.Blob.Name = client.Name;

        return response;
    }

    public async Task<List<BlobDto>> ListAsync() {
        var cloudBlobContainer = await GetStorageClient();
        List<BlobDto> files = new();

        await foreach (var file in cloudBlobContainer.GetBlobsAsync()) {
            string uri = cloudBlobContainer.Uri.ToString();
            var name = file.Name;
            var fullUri = $"{uri}/{name}";

            files.Add(new BlobDto {
                Uri = fullUri,
                Name = name,
                ContentType = file.Properties.ContentType
            });
        }
        return files;
    }

    public async Task<BlobDto> DownloadAsync(string blobFilename) {
        var cloudBlobContainer = await GetStorageClient();
        var file = cloudBlobContainer.GetBlobClient(blobFilename);

        if (await file.ExistsAsync()) {
            var data = await file.OpenReadAsync();
            Stream blobContent = data;

            var content = await file.DownloadContentAsync();

            string name = blobFilename;
            string contentType = content.Value.Details.ContentType;

            return new BlobDto {
                Content = blobContent,
                Name = name,
                ContentType = contentType
            };
        }
        return null;
    }

    public async Task<BlobResponseDto> DeleteAsync(string blobFilename) {
        var cloudBlobContainer = await GetStorageClient();
        var file = cloudBlobContainer.GetBlobClient(blobFilename);

        var response =  await file.DeleteIfExistsAsync();

        return new BlobResponseDto {
            Error = false,
            Status = $"File: {blobFilename} has been successffully deleted"
        };
    }

    // IStorageService implementation
    async Task<string> IStorageService.UploadAsync(Stream fileStream, string fileName, string contentType, CancellationToken cancellationToken)
    {
        var cloudBlobContainer = await GetStorageClient();
        var uniqueFileName = UniqueFileName(fileName);
        var client = cloudBlobContainer.GetBlobClient(uniqueFileName);

        await client.UploadAsync(fileStream, cancellationToken);
        return uniqueFileName;
    }

    async Task<Stream> IStorageService.DownloadAsync(string fileName, CancellationToken cancellationToken)
    {
        var cloudBlobContainer = await GetStorageClient();
        var file = cloudBlobContainer.GetBlobClient(fileName);

        if (await file.ExistsAsync(cancellationToken))
        {
            return await file.OpenReadAsync(cancellationToken: cancellationToken);
        }

        throw new FileNotFoundException($"File '{fileName}' not found in storage");
    }

    async Task<bool> IStorageService.DeleteAsync(string fileName, CancellationToken cancellationToken)
    {
        var cloudBlobContainer = await GetStorageClient();
        var file = cloudBlobContainer.GetBlobClient(fileName);
        return await file.DeleteIfExistsAsync(cancellationToken: cancellationToken);
    }

    async Task<bool> IStorageService.ExistsAsync(string fileName, CancellationToken cancellationToken)
    {
        var cloudBlobContainer = await GetStorageClient();
        var file = cloudBlobContainer.GetBlobClient(fileName);
        return await file.ExistsAsync(cancellationToken);
    }

    async Task<string> IStorageService.GetDownloadUrlAsync(string fileName, TimeSpan expiresIn, CancellationToken cancellationToken)
    {
        var cloudBlobContainer = await GetStorageClient();
        var file = cloudBlobContainer.GetBlobClient(fileName);

        if (!await file.ExistsAsync(cancellationToken))
        {
            throw new FileNotFoundException($"File '{fileName}' not found in storage");
        }

        var sasBuilder = new BlobSasBuilder
        {
            BlobContainerName = _containerName,
            BlobName = fileName,
            Resource = "b",
            StartsOn = DateTimeOffset.UtcNow,
            ExpiresOn = DateTimeOffset.UtcNow.Add(expiresIn)
        };

        sasBuilder.SetPermissions(BlobSasPermissions.Read);

        return file.GenerateSasUri(sasBuilder).ToString();
    }

    async Task<IEnumerable<string>> IStorageService.ListFilesAsync(string? prefix, CancellationToken cancellationToken)
    {
        var cloudBlobContainer = await GetStorageClient();
        var files = new List<string>();

        await foreach (var file in cloudBlobContainer.GetBlobsAsync(prefix: prefix, cancellationToken: cancellationToken))
        {
            files.Add(file.Name);
        }

        return files;
    }
}
