namespace Temp.Services.Integrations.Azure.AzureStorage.Models;

public class BlobResponseDto
{
    public BlobResponseDto() {
        Blob = new();
    }

    public string? Status { get; set; }
    public bool Error { get; set; }
    public string? ErrorMessage { get; set; }
    public string? ErrorCode { get; set; }
    public BlobDto Blob { get; set; }

    public static BlobResponseDto Success(BlobDto blob, string status) => new() {
        Blob = blob,
        Status = status,
        Error = false
    };

    public static BlobResponseDto Failure(string errorMessage, string? errorCode = null) => new() {
        Error = true,
        ErrorMessage = errorMessage,
        ErrorCode = errorCode
    };
}