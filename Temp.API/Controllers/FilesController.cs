using Temp.Services.Integrations.Azure.AzureStorage;

namespace Temp.API.Controllers;

[Authorize(Roles = "Admin")]
[ApiController]
[Route("api/[controller]")]
public class FilesController : ControllerBase
{
    private readonly IAzureStorageService _azureStorageService;
    private static readonly string[] AllowedExtensions = { ".jpg", ".jpeg", ".png", ".gif", ".pdf", ".doc", ".docx" };
    private const long MaxFileSize = 10 * 1024 * 1024;

    public FilesController(IAzureStorageService azureStorageService) {
        _azureStorageService = azureStorageService;
    }

    [HttpGet]
    public async Task<IActionResult> ListAllBlobs() {
        var result = await _azureStorageService.ListAsync();
        return Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> Upload(IFormFile file) {

        if (file == null || file.Length == 0)
            return BadRequest("No file provided");

        if (file.Length > MaxFileSize)
            return BadRequest($"File size exceeds maximum allowed size of {MaxFileSize / 1024 / 1024}MB");

        var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
        if (string.IsNullOrEmpty(extension) || !AllowedExtensions.Contains(extension))
            return BadRequest($"File type '{extension}' is not allowed. Allowed types: {string.Join(", ", AllowedExtensions)}");

        var result = await _azureStorageService.UploadAsync(file);
        return Ok(result);
    }

    [HttpGet]
    [Route("filename")]
    public async Task<IActionResult> Download(string filename) {

        if (!IsValidFilename(filename))
            return BadRequest("Invalid filename");

        var result = await _azureStorageService.DownloadAsync(filename);

        return File(result.Content, result.ContentType, result.Name);
    }

    [HttpDelete]
    [Route("filename")]
    public async Task<IActionResult> Delete(string filename) {

        if (!IsValidFilename(filename))
            return BadRequest("Invalid filename");

        var result = await _azureStorageService.DeleteAsync(filename);
        return Ok(result);
    }

    private static bool IsValidFilename(string filename) {
        if (string.IsNullOrWhiteSpace(filename))
            return false;


        if (filename.Contains("..") || filename.Contains("/") || filename.Contains("\\"))
            return false;


        if (Path.GetFileName(filename) != filename)
            return false;

        return true;
    }
}