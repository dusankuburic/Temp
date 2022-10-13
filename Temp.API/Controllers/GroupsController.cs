using Temp.Core.Groups;
using Temp.Domain.Models.Groups.Exceptions;

namespace Temp.API.Controllers;

[Authorize(Roles = "Admin, Moderator")]
[Route("api/[controller]")]
[ApiController]
public class GroupsController : ControllerBase
{
    private readonly ApplicationDbContext _ctx;

    public GroupsController(ApplicationDbContext ctx) {
        _ctx = ctx;
    }


    [HttpPost]
    public async Task<IActionResult> Create(CreateGroup.Request request) {
        try {
            var response = await new CreateGroup(_ctx).Do(request);
            if (response.Status)
                return NoContent();

            return BadRequest(response.Message);
        } catch (GroupValidationException groupValidationException) {
            return BadRequest(GetInnerMessage(groupValidationException));
        }
    }


    [HttpGet("{id}")]
    public async Task<IActionResult> GetGroup(int id) {
        try {
            var group = await new GetGroup(_ctx).Do(id);
            return Ok(group);
        } catch (GroupValidationException groupValidationException) {
            return BadRequest(GetInnerMessage(groupValidationException));
        }
    }


    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateGroup(int id, UpdateGroup.Request request) {
        try {
            var response = await new UpdateGroup(_ctx).Do(id, request);
            if (response.Status)
                return NoContent();

            return BadRequest(response.Message);
        } catch (GroupValidationException groupValidationException) {
            return BadRequest(GetInnerMessage(groupValidationException));
        }
    }


    [HttpGet("inner-teams/{id}")]
    public async Task<IActionResult> InnerTeams(int id) {
        try {
            var response = await new GetInnerTeams(_ctx).Do(id);
            return Ok(response);
        } catch (GroupValidationException groupValidationException) {
            return BadRequest(GetInnerMessage(groupValidationException));
        }
    }


    [HttpGet("moderator-groups/{id}")]
    public async Task<IActionResult> GetModeratorGroups(int id) {
        try {
            var response = await new GetModeratorGroups(_ctx).Do(id);
            return Ok(response);
        } catch (GroupValidationException groupValidationException) {
            return BadRequest(GetInnerMessage(groupValidationException));
        }
    }

    [HttpGet("moderator-free-groups/{organizationId}/moderator/{moderatorId}")]
    public async Task<IActionResult> GetModeratorFreeGroups(int organizationId, int moderatorId) {
        try {
            var response = await new GetModeratorFreeGroups(_ctx).Do(organizationId, moderatorId);
            return Ok(response);
        } catch (GroupValidationException groupValidationException) {
            return BadRequest(GetInnerMessage(groupValidationException));
        }
    }

    [HttpPut("change-status/{id}")]
    public async Task<IActionResult> UpdateGroupStatus(int id) {
        var response = await new UpdateGroupStatus(_ctx).Do(id);
        if (response)
            return NoContent();

        return BadRequest();
    }

    private static string GetInnerMessage(Exception exception) =>
        exception.InnerException.Message;
}
