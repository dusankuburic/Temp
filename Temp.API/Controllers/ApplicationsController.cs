using Temp.Services.Applications;
using Temp.Services.Applications.Exceptions;
using Temp.Services.Applications.Models.Commands;
using Temp.Services.Applications.Models.Queries;

namespace Temp.API.Controllers;

[Authorize(Roles = "Moderator, User, Admin")]
[Route("api/[controller]")]
[ApiController]
public class ApplicationsController : ControllerBase
{
    private readonly IApplicationService _applicationService;

    public ApplicationsController(IApplicationService applicationService) {
        _applicationService = applicationService;
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateApplicationRequest request) {
        try {
            var response = await _applicationService.CreateApplication(request);

            return Ok(response);
        } catch (ApplicationValidationException applicationValidationException) {
            return BadRequest(GetInnerMessage(applicationValidationException));
        }
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetApplication([FromRoute] GetApplicationRequest request) {
        try {
            var response = await _applicationService.GetApplication(request);

            return Ok(response);
        } catch (ApplicationValidationException applicationValidationException) {
            return BadRequest(GetInnerMessage(applicationValidationException));
        }
    }

    [HttpGet("team/{teamId}/moderator/{moderatorId}")]
    public async Task<IActionResult> GetTeamApplications([FromRoute] GetTeamApplicationsRequest request) {
        try {
            var response = await _applicationService.GetTeamApplications(request);

            return Ok(response);
        } catch (ApplicationValidationException applicationValidationException) {
            return BadRequest(GetInnerMessage(applicationValidationException));
        }
    }

    [HttpGet("user/{id}")]
    public async Task<IActionResult> GetUserApplications([FromRoute] GetUserApplicationsRequest request) {
        try {
            var response = await _applicationService.GetUserApplications(request);

            return Ok(response);
        } catch (ApplicationValidationException applicationValidationException) {
            return BadRequest(GetInnerMessage(applicationValidationException));
        }
    }

    [HttpPut("change-status/{id}")]
    public async Task<IActionResult> UpdateApplicationStatus([FromBody] UpdateApplicationStatusRequest request) {
        try {
            var response = await _applicationService.UpdateApplicationStatus(request);

            return NoContent();
        } catch (ApplicationServiceException applicationValidationException) {
            return BadRequest(GetInnerMessage(applicationValidationException));
        }
    }

    private static string GetInnerMessage(Exception exception) =>
        exception.InnerException.Message;
}
