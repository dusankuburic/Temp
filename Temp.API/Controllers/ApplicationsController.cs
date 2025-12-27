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
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(CreateApplicationResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> Create([FromBody] CreateApplicationRequest request) {
        var response = await _applicationService.CreateApplication(request);

        return Ok(response);
    }

    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(GetApplicationResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetApplication([FromRoute] GetApplicationRequest request) {
        var response = await _applicationService.GetApplication(request);

        return Ok(response);
    }

    [HttpGet("team/{teamId}/moderator/{moderatorId}")]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(IEnumerable<GetTeamApplicationsResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetTeamApplications([FromRoute] GetTeamApplicationsRequest request) {
        var response = await _applicationService.GetTeamApplications(request);

        return Ok(response);
    }

    [HttpGet("user/{id}")]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(IEnumerable<GetApplicationResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetUserApplications([FromRoute] GetUserApplicationsRequest request) {
        var response = await _applicationService.GetUserApplications(request);

        return Ok(response);
    }

    [HttpPut("change-status/{id}")]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> UpdateApplicationStatus([FromBody] UpdateApplicationStatusRequest request) {
        var response = await _applicationService.UpdateApplicationStatus(request);

        return NoContent();
    }

}