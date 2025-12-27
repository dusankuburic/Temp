namespace Temp.Services.Integrations.Azure.AzureStorage.Models;

public class BlobResponseDto
{
    public BlobResponseDto() {
        Blob = new();
    }

    public string Status { get; set; }
    public bool Error { get; set; }
    public BlobDto Blob { get; set; }
}