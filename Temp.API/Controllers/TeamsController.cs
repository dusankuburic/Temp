using Temp.Services.Teams;
using Temp.Services.Teams.Exceptions;
using Temp.Services.Teams.Models.Commands;

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
    public async Task<IActionResult> Create(CreateTeam.Request request) {
        try {
            var response = await _teamService.CreateTeam(request);
            return response.Status ? NoContent() : BadRequest(response.Message);
        } catch (TeamValidationException teamValidationException) {
            return BadRequest(GetInnerMessage(teamValidationException));
        }
    }

    [Authorize(Roles = "Admin")]
    [HttpGet("{id}")]
    public async Task<IActionResult> GetTeam(int id) {
        try {
            var response = await _teamService.GetTeam(id);
            return Ok(response);
        } catch (TeamValidationException teamValidationException) {
            return BadRequest(GetInnerMessage(teamValidationException));
        }
    }

    [Authorize(Roles = "Admin")]
    [HttpGet("full/{id}")]
    public async Task<IActionResult> GetFullTeam(int id) {
        try {
            var response = await _teamService.GetFullTeamTree(id);
            return Ok(response);
        } catch (TeamValidationException teamValidationException) {
            return BadRequest(GetInnerMessage(teamValidationException));
        }
    }

    [HttpGet("employee/team/{userId}")]
    public async Task<IActionResult> GetUserTeam(int userId) {
        try {
            var response = await _teamService.GetUserTeam(userId);
            return Ok(response);
        } catch (TeamValidationException teamValidationException) {
            return BadRequest(GetInnerMessage(teamValidationException));
        }
    }

    [Authorize(Roles = "Admin")]
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateTeam(int id, UpdateTeam.Request request) {
        try {
            var response = await _teamService.UpdateTeam(id, request);
            return response.Status ? NoContent() : BadRequest(response.Message);
        } catch (TeamValidationException teamValidationException) {
            return BadRequest(GetInnerMessage(teamValidationException));
        }
    }

    [HttpPut("change-status/{id}")]
    public async Task<IActionResult> UpdateTeamStatus(int id) {
        var response = await _teamService.UpdateTeamStatus(id);
        return response ? NoContent() : BadRequest();
    }

    private static string GetInnerMessage(Exception exception) {
        return exception.InnerException.Message;
    }
}
