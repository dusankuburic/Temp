using Temp.Services.Teams;
using Temp.Services.Teams.Exceptions;
using Temp.Services.Teams.Models.Commands;
using Temp.Services.Teams.Models.Queries;

namespace Temp.API.Controllers;

[Authorize(Roles = "Admin, User")]
[Route("api/[controller]")]
[ApiController]
public class TeamsController : ControllerBase
{
    private readonly ITeamService _teamService;

    public TeamsController(ITeamService teamService) {
        _teamService = teamService;
    }

    [Authorize(Roles = "Admin")]
    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(GetTeamResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetTeam([FromRoute] GetTeamRequest request) {
        try {
            var response = await _teamService.GetTeam(request);

            return Ok(response);
        } catch (TeamValidationException teamValidationException) {
            return BadRequest(GetInnerMessage(teamValidationException));
        }
    }

    [Authorize(Roles = "Admin")]
    [HttpGet("full/{id}")]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(GetFullTeamTreeResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetFullTeam([FromRoute] GetFullTeamTreeRequest request) {
        try {
            var response = await _teamService.GetFullTeamTree(request);

            return Ok(response);
        } catch (TeamValidationException teamValidationException) {
            return BadRequest(GetInnerMessage(teamValidationException));
        }
    }

    [HttpGet("employee/team/{id}")]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(GetUserTeamResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetUserTeam([FromRoute] GetUserTeamRequest request) {
        try {
            var response = await _teamService.GetUserTeam(request);

            return Ok(response);
        } catch (TeamValidationException teamValidationException) {
            return BadRequest(GetInnerMessage(teamValidationException));
        }
    }

    [Authorize(Roles = "Admin")]
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(CreateTeamResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> Create([FromBody] CreateTeamRequest request) {
        try {
            var response = await _teamService.CreateTeam(request);

            return Ok(response);
        } catch (TeamValidationException teamValidationException) {
            return BadRequest(GetInnerMessage(teamValidationException));
        }
    }

    [Authorize(Roles = "Admin")]
    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(UpdateTeamResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> UpdateTeam([FromBody] UpdateTeamRequest request) {
        try {
            var response = await _teamService.UpdateTeam(request);

            return NoContent();
        } catch (TeamValidationException teamValidationException) {
            return BadRequest(GetInnerMessage(teamValidationException));
        }
    }

    [HttpPut("change-status/{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> UpdateTeamStatus([FromRoute] UpdateTeamStatusRequest request) {
        var response = await _teamService.UpdateTeamStatus(request);

        return NoContent();
    }

    [HttpGet("team-exists")]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
    public async Task<IActionResult> TeamExists([FromQuery] string name, int groupId) {
        try {
            var response = await _teamService.TeamExists(name, groupId);

            return Ok(response);
        } catch (TeamValidationException teamValidationException) {
            return BadRequest(GetInnerMessage(teamValidationException));
        }
    }

    private static string GetInnerMessage(Exception exception) {
        return exception.InnerException.Message;
    }
}
