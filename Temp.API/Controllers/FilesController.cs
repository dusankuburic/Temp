using Temp.Services.Integrations.Azure.AzureStorage;

namespace Temp.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class FilesController : ControllerBase
{
    private readonly IAzureStorageService _azureStorageService;

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

        var result = await _azureStorageService.UploadAsync(file);
        return Ok(result);
    }

    [HttpGet]
    [Route("filename")]
    public async Task<IActionResult> Download(string filename) {
        var result = await _azureStorageService.DownloadAsync(filename);

        return File(result.Content, result.ContentType, result.Name);
    }

    [HttpDelete]
    [Route("filename")]
    public async Task<IActionResult> Delete(string filename) {
        var result = await _azureStorageService.DeleteAsync(filename);
        return Ok(result);
    }
}

