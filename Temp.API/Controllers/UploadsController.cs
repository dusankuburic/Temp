using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Temp.Services.Integrations.Azure.AzureStorage;
using Temp.Services.Integrations.Azure.AzureStorage.Models;

namespace Temp.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class UploadsController : ControllerBase
    {
        private readonly IAzureStorageService _blobStorageService;

        public UploadsController(IAzureStorageService blobStorageService)
        {
            _blobStorageService = blobStorageService;
        }

        [HttpPost("signed-url")]
        public async Task<ActionResult<object>> GetSignedUrl([FromBody] UploadRequest request, CancellationToken ct)
        {
            if (string.IsNullOrWhiteSpace(request.Filename))
            {
                return BadRequest("Filename is required.");
            }

            try
            {
                var sasUrl = await _blobStorageService.GenerateSasTokenForUploadAsync(request.Filename, request.FileType, ct);
                return Ok(new { url = sasUrl });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }

    public class UploadRequest
    {
        public string Filename { get; set; } = string.Empty;
        public FileType FileType { get; set; } = FileType.Image;
    }
}
