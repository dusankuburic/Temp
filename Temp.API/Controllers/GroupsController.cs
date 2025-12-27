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
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(GetGroupResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetGroup([FromRoute] GetGroupRequest request) {
        var group = await _groupService.GetGroup(request);

        return Ok(group);
    }

    [HttpGet("moderator-groups/{id}")]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(List<GetModeratorGroupsResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetModeratorGroups([FromRoute] GetModeratorGroupsRequest request) {
        var response = await _groupService.GetModeratorGroups(request);

        return Ok(response);
    }

    [HttpGet("moderator-free-groups/{organizationId}/moderator/{moderatorId}")]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(List<GetModeratorFreeGroupsResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetModeratorFreeGroups([FromRoute] GetModeratorFreeGroupsRequest request) {
        var response = await _groupService.GetModeratorFreeGroups(request);

        return Ok(response);
    }

    [HttpGet("paged-inner-teams")]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(GetPagedGroupInnerTeamsResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetPagedInnerTeams([FromQuery] GetPagedGroupInnerTeamsRequest request) {
        var response = await _groupService.GetPagedGroupInnerTeams(request);
        Response.AddPagination(response.Teams.CurrentPage, response.Teams.PageSize, response.Teams.TotalCount, response.Teams.TotalPages);

        return Ok(response);
    }

    [HttpGet("inner-teams/{id}")]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(List<InnerTeam>), StatusCodes.Status200OK)]
    public async Task<IActionResult> InnerTeams([FromRoute] int id) {
        var response = await _groupService.GetGroupInnerTeams(id);

        return Ok(response);
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(CreateGroupResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> Create([FromBody] CreateGroupRequest request) {
        var response = await _groupService.CreateGroup(request);

        return Ok(response);
    }

    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> UpdateGroup([FromBody] UpdateGroupRequest request) {
        var response = await _groupService.UpdateGroup(request);

        return NoContent();
    }

    [HttpPut("change-status/{id}")]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> UpdateGroupStatus([FromRoute] UpdateGroupStatusRequest request) {
        var response = await _groupService.UpdateGroupStatus(request);

        return NoContent();
    }

    [HttpGet("group-exists")]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
    public async Task<IActionResult> GroupExists([FromQuery] string name, int organizationId) {
        var response = await _groupService.GroupExists(name, organizationId);

        return Ok(response);
    }

}