using Temp.Services.Groups;
using Temp.Services.Groups.Exceptions;
using Temp.Services.Groups.Models.Command;
using Temp.Services.Groups.Models.Query;

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

    [HttpPost]
    public async Task<IActionResult> Create(CreateGroupRequest request) {
        try {
            var response = await _groupService.CreateGroup(request);

            return Ok(response);
        } catch (GroupValidationException groupValidationException) {
            return BadRequest(GetInnerMessage(groupValidationException));
        }
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetGroup([FromQuery] GetGroupRequest request) {
        try {
            var group = await _groupService.GetGroup(request);
            return Ok(group);
        } catch (GroupValidationException groupValidationException) {
            return BadRequest(GetInnerMessage(groupValidationException));
        }
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateGroup(UpdateGroupRequest request) {
        try {
            var response = await _groupService.UpdateGroup(request);
            return Ok(response);
        } catch (GroupValidationException groupValidationException) {
            return BadRequest(GetInnerMessage(groupValidationException));
        }
    }

    [HttpGet("inner-teams/{id}")]
    public async Task<IActionResult> InnerTeams([FromQuery] GetGroupInnerTeamsRequest request) {
        try {
            var response = await _groupService.GetGroupInnerTeams(request);
            return Ok(response);
        } catch (GroupValidationException groupValidationException) {
            return BadRequest(GetInnerMessage(groupValidationException));
        }
    }

    [HttpGet("moderator-groups/{id}")]
    public async Task<IActionResult> GetModeratorGroups([FromQuery] GetModeratorGroupsRequest request) {
        try {
            var response = await _groupService.GetModeratorGroups(request);
            return Ok(response);
        } catch (GroupValidationException groupValidationException) {
            return BadRequest(GetInnerMessage(groupValidationException));
        }
    }

    [HttpGet("moderator-free-groups/{organizationId}/moderator/{moderatorId}")]
    public async Task<IActionResult> GetModeratorFreeGroups([FromQuery] GetModeratorFreeGroupsRequest request) {
        try {
            var response = await _groupService.GetModeratorFreeGroups(request);
            return Ok(response);
        } catch (GroupValidationException groupValidationException) {
            return BadRequest(GetInnerMessage(groupValidationException));
        }
    }

    [HttpPut("change-status/{id}")]
    public async Task<IActionResult> UpdateGroupStatus([FromQuery] UpdateGroupStatusRequest request) {
        try {
            var response = await _groupService.UpdateGroupStatus(request);
            return Ok(response);
        } catch (GroupValidationException groupValidationException) {
            return BadRequest(GetInnerMessage(groupValidationException));
        }
    }

    private static string GetInnerMessage(Exception exception) {
        return exception.InnerException.Message;
    }
}
