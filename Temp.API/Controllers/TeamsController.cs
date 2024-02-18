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
    [HttpPost]
    public async Task<IActionResult> Create(CreateTeamRequest request) {
        try {
            var response = await _teamService.CreateTeam(request);

            return Ok(response);
        } catch (TeamValidationException teamValidationException) {
            return BadRequest(GetInnerMessage(teamValidationException));
        }
    }

    [Authorize(Roles = "Admin")]
    [HttpGet("{id}")]
    public async Task<IActionResult> GetTeam([FromRoute] GetTeamRequest request) {
        try {
            var response = await _teamService.GetTeam(request);
            return Ok(response);
        } catch (TeamValidationException teamValidationException) {
            return BadRequest(GetInnerMessage(teamValidationException));
        }
    }

    [Authorize(Roles = "Admin")]
    [HttpGet("full/{Id}")]
    public async Task<IActionResult> GetFullTeam([FromRoute] GetFullTeamTreeRequest request) {
        try {
            var response = await _teamService.GetFullTeamTree(request);
            return Ok(response);
        } catch (TeamValidationException teamValidationException) {
            return BadRequest(GetInnerMessage(teamValidationException));
        }
    }

    [HttpGet("employee/team/{Id}")]
    public async Task<IActionResult> GetUserTeam([FromRoute] GetUserTeamRequest request) {
        try {
            var response = await _teamService.GetUserTeam(request);
            return Ok(response);
        } catch (TeamValidationException teamValidationException) {
            return BadRequest(GetInnerMessage(teamValidationException));
        }
    }

    [Authorize(Roles = "Admin")]
    [HttpPut]
    public async Task<IActionResult> UpdateTeam(UpdateTeamRequest request) {
        try {
            var response = await _teamService.UpdateTeam(request);

            return Ok(response);
        } catch (TeamValidationException teamValidationException) {
            return BadRequest(GetInnerMessage(teamValidationException));
        }
    }

    [HttpPut("change-status")]
    public async Task<IActionResult> UpdateTeamStatus(UpdateTeamStatusRequest request) {
        var response = await _teamService.UpdateTeamStatus(request);

        return Ok(response);
    }

    private static string GetInnerMessage(Exception exception) {
        return exception.InnerException.Message;
    }
}
