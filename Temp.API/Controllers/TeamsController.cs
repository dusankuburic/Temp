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
        var response = await _teamService.GetTeam(request);

        return Ok(response);
    }

    [Authorize(Roles = "Admin")]
    [HttpGet("full/{id}")]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(GetFullTeamTreeResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetFullTeam([FromRoute] GetFullTeamTreeRequest request) {
        var response = await _teamService.GetFullTeamTree(request);

        return Ok(response);
    }

    [HttpGet("employee/team/{id}")]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(GetUserTeamResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetUserTeam([FromRoute] GetUserTeamRequest request) {
        var response = await _teamService.GetUserTeam(request);

        return Ok(response);
    }

    [Authorize(Roles = "Admin")]
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(CreateTeamResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> Create([FromBody] CreateTeamRequest request) {
        var response = await _teamService.CreateTeam(request);

        return Ok(response);
    }

    [Authorize(Roles = "Admin")]
    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(UpdateTeamResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> UpdateTeam([FromBody] UpdateTeamRequest request) {
        var response = await _teamService.UpdateTeam(request);

        return NoContent();
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
        var response = await _teamService.TeamExists(name, groupId);

        return Ok(response);
    }

    [Authorize(Roles = "Admin")]
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteTeam([FromRoute] int id) {
        await _teamService.DeleteTeamAsync(id);
        return NoContent();
    }

    [Authorize]
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(IEnumerable<GetTeamResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetTeams() {
        var response = await _teamService.GetAllTeamsAsync();
        return Ok(response);
    }

}