using Temp.Services.Groups;
using Temp.Services.Groups.Exceptions;
using Temp.Services.Groups.Models.Commands;
using Temp.Services.Groups.Models.Queries;

namespace Temp.API.Controllers;

[Authorize(Roles = "Admin, Moderator")]
[Route("api/[controller]")]
[ApiController]
public class GroupsController : ControllerBase
{
    private readonly IGroupService _groupService;

    public GroupsController(IGroupService groupService) {
        _groupService = groupService;
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetGroup([FromRoute] GetGroupRequest request) {
        try {
            var group = await _groupService.GetGroup(request);
            return Ok(group);
        } catch (GroupValidationException groupValidationException) {
            return BadRequest(GetInnerMessage(groupValidationException));
        }
    }

    [HttpGet("moderator-groups/{id}")]
    public async Task<IActionResult> GetModeratorGroups([FromRoute] GetModeratorGroupsRequest request) {
        try {
            var response = await _groupService.GetModeratorGroups(request);

            return Ok(response);
        } catch (GroupValidationException groupValidationException) {
            return BadRequest(GetInnerMessage(groupValidationException));
        }
    }

    [HttpGet("moderator-free-groups/{organizationId}/moderator/{moderatorId}")]
    public async Task<IActionResult> GetModeratorFreeGroups([FromRoute] GetModeratorFreeGroupsRequest request) {
        try {
            var response = await _groupService.GetModeratorFreeGroups(request);

            return Ok(response);
        } catch (GroupValidationException groupValidationException) {
            return BadRequest(GetInnerMessage(groupValidationException));
        }
    }

    [HttpGet("inner-teams/{id}")]
    public async Task<IActionResult> InnerTeams([FromRoute] GetGroupInnerTeamsRequest request) {
        try {
            var response = await _groupService.GetGroupInnerTeams(request);

            return Ok(response);
        } catch (GroupValidationException groupValidationException) {
            return BadRequest(GetInnerMessage(groupValidationException));
        }
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateGroupRequest request) {
        try {
            var response = await _groupService.CreateGroup(request);

            return Ok(response);
        } catch (GroupValidationException groupValidationException) {
            return BadRequest(GetInnerMessage(groupValidationException));
        }
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateGroup([FromBody] UpdateGroupRequest request) {
        try {
            var response = await _groupService.UpdateGroup(request);

            return NoContent();
        } catch (GroupValidationException groupValidationException) {
            return BadRequest(GetInnerMessage(groupValidationException));
        }
    }

    [HttpPut("change-status/{id}")]
    public async Task<IActionResult> UpdateGroupStatus([FromBody] UpdateGroupStatusRequest request) {
        try {
            var response = await _groupService.UpdateGroupStatus(request);

            return NoContent();
        } catch (GroupValidationException groupValidationException) {
            return BadRequest(GetInnerMessage(groupValidationException));
        }
    }

    private static string GetInnerMessage(Exception exception) {
        return exception.InnerException.Message;
    }
}
