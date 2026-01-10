namespace Temp.Services.Integrations.Azure.AzureStorage;

public class AzureStorageOptions
{
    public const string SectionName = "AzureStorage";

    public string ContainerName { get; set; } = "files";
    public long MaxFileSizeBytes { get; set; } = 10 * 1024 * 1024; // 10MB
    public string[] AllowedImageExtensions { get; set; } = [".jpg", ".jpeg", ".png", ".gif", ".webp", ".bmp"];
    public string[] AllowedDocumentExtensions { get; set; } = [".pdf", ".doc", ".docx", ".xls", ".xlsx", ".txt"];
    public string? PublicHost { get; set; }
}
