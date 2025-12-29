using Microsoft.AspNetCore.Mvc;
using Temp.Services.Integrations.Azure.AzureStorage;

namespace Temp.API.Controllers
{
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
        public ActionResult<object> GetSignedUrl([FromBody] UploadRequest request)
        {
            if (string.IsNullOrEmpty(request.Filename))
            {
                return BadRequest("Filename is required.");
            }

            var sasUrl = _blobStorageService.GenerateSasTokenForUpload(request.Filename);
            return Ok(new { url = sasUrl });
        }
    }

    public class UploadRequest
    {
        public string Filename { get; set; }
    }
}
