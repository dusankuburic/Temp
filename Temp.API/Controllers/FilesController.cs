using Temp.Services.Integrations.Azure.AzureStorage;
using Temp.Services.Integrations.Azure.AzureStorage.Models;

namespace Temp.API.Controllers;

[Authorize(Roles = "Admin")]
[ApiController]
[Route("api/[controller]")]
public class FilesController : ControllerBase
{
    private readonly IAzureStorageService _azureStorageService;

    public FilesController(IAzureStorageService azureStorageService) {
        _azureStorageService = azureStorageService;
    }

    [HttpGet]
    public async Task<IActionResult> ListAllBlobs(CancellationToken ct) {
        var result = await _azureStorageService.ListAsync(ct);
        return Ok(result);
    }

    [HttpGet("images")]
    public async Task<IActionResult> ListImages(
        [FromQuery] DateTime? from,
        [FromQuery] DateTime? to,
        [FromQuery] string? folder,
        CancellationToken ct) {
        var result = await _azureStorageService.ListAsync(FileType.Image, from, to, folder, ct);
        return Ok(result);
    }

    [HttpGet("documents")]
    public async Task<IActionResult> ListDocuments(
        [FromQuery] DateTime? from,
        [FromQuery] DateTime? to,
        [FromQuery] string? folder,
        CancellationToken ct) {
        var result = await _azureStorageService.ListAsync(FileType.Document, from, to, folder, ct);
        return Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> Upload(IFormFile file, CancellationToken ct) {
        if (file == null || file.Length == 0)
            return BadRequest("No file provided");

        var result = await _azureStorageService.UploadAsync(file, ct);
        return result.Error ? BadRequest(result) : Ok(result);
    }

    [HttpPost("images")]
    public async Task<IActionResult> UploadImage(
        IFormFile file,
        [FromQuery] DateTime? createdAt,
        [FromQuery] string? folder,
        CancellationToken ct) {
        if (file == null || file.Length == 0)
            return BadRequest("No file provided");

        var result = await _azureStorageService.UploadImageAsync(file, createdAt, folder, ct);
        return result.Error ? BadRequest(result) : Ok(result);
    }

    [HttpPost("documents")]
    public async Task<IActionResult> UploadDocument(
        IFormFile file,
        [FromQuery] DateTime? createdAt,
        [FromQuery] string? folder,
        CancellationToken ct) {
        if (file == null || file.Length == 0)
            return BadRequest("No file provided");

        var result = await _azureStorageService.UploadDocumentAsync(file, createdAt, folder, ct);
        return result.Error ? BadRequest(result) : Ok(result);
    }

    [HttpGet("download")]
    public async Task<IActionResult> Download([FromQuery] string path, CancellationToken ct) {
        if (string.IsNullOrWhiteSpace(path))
            return BadRequest("Path is required");

        var result = await _azureStorageService.DownloadAsync(path, ct);

        if (result == null)
            return NotFound($"File not found: {path}");

        return File(result.Content!, result.ContentType!, Path.GetFileName(result.Name));
    }

    [HttpGet("url")]
    public async Task<IActionResult> GetDownloadUrl(
        [FromQuery] string path,
        [FromQuery] int expiresInMinutes = 60,
        CancellationToken ct = default) {
        if (string.IsNullOrWhiteSpace(path))
            return BadRequest("Path is required");

        try {
            var url = await _azureStorageService.GetDownloadUrlAsync(path, TimeSpan.FromMinutes(expiresInMinutes), ct);
            return Ok(new { url });
        }
        catch (FileNotFoundException) {
            return NotFound($"File not found: {path}");
        }
    }

    [HttpDelete]
    public async Task<IActionResult> Delete([FromQuery] string path, CancellationToken ct) {
        if (string.IsNullOrWhiteSpace(path))
            return BadRequest("Path is required");

        var result = await _azureStorageService.DeleteAsync(path, ct);
        return result.Error ? NotFound(result) : Ok(result);
    }

    [HttpGet("exists")]
    public async Task<IActionResult> Exists([FromQuery] string path, CancellationToken ct) {
        if (string.IsNullOrWhiteSpace(path))
            return BadRequest("Path is required");

        var exists = await _azureStorageService.ExistsAsync(path, ct);
        return Ok(new { exists });
    }
}
