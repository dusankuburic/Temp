using Temp.Services.Applications;
using Temp.Services.Applications.Exceptions;
using Temp.Services.Applications.Models.Commands;

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
    public async Task<IActionResult> Create(CreateApplication.Request request) {
        try {
            var response = await _applicationService.CreateApplication(request);
            if (response.Status)
                return NoContent();

            return BadRequest("Error");
        } catch (ApplicationValidationException applicationValidationException) {
            return BadRequest(GetInnerMessage(applicationValidationException));
        }
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetApplication(int id) {
        try {
            var response = await _applicationService.GetApplication(id);
            return Ok(response);
        } catch (ApplicationValidationException applicationValidationException) {
            return BadRequest(GetInnerMessage(applicationValidationException));
        }
    }

    [HttpGet("team/{teamId}/moderator/{moderatorId}")]
    public async Task<IActionResult> GetTeamApplications(int teamId, int moderatorId) {
        try {
            var response = await _applicationService.GetTeamApplications(teamId, moderatorId);
            return Ok(response);
        } catch (ApplicationValidationException applicationValidationException) {
            return BadRequest(GetInnerMessage(applicationValidationException));
        }
    }

    [HttpGet("user/{id}")]
    public async Task<IActionResult> GetUserApplications(int id) {
        try {
            var response = await _applicationService.GetUserApplications(id);
            return Ok(response);
        } catch (ApplicationValidationException applicationValidationException) {
            return BadRequest(GetInnerMessage(applicationValidationException));
        }
    }

    [HttpPut("change-status/{id}")]
    public async Task<IActionResult> UpdateApplicationStatus(int id, UpdateApplicationStatus.Request request) {
        try {
            var response = await _applicationService.UpdateApplicationStatus(id, request);
            if (response.Status)
                return NoContent();

            return BadRequest();
        } catch (ApplicationServiceException applicationValidationException) {
            return BadRequest(GetInnerMessage(applicationValidationException));
        }
    }

    private static string GetInnerMessage(Exception exception) =>
        exception.InnerException.Message;
}
