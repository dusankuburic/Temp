using Temp.Domain.Models.ModeratorGroups.Exceptions;
using Temp.Services.Auth.Moderators;

namespace Temp.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ModeratorsController : ControllerBase
{
    private readonly ApplicationDbContext _ctx;

    public ModeratorsController(ApplicationDbContext ctx) {
        _ctx = ctx;
    }

    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(GetModerator.ModeratorViewModel), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetModerator([FromRoute] int id) {
        var response = await new GetModerator(_ctx).Do(id);

        return Ok(response);
    }

    [Authorize(Roles = "Admin")]
    [HttpPut("update-groups/{id}")]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> UpdateGroups(int id, UpdateModeratorGroupsRequest request) {
        try {
            var response = await new UpdateModeratorGroups(_ctx).Do(id, request);
            if (response.Status)
                return NoContent();

            return BadRequest(response.Message);
        } catch (ModeratorGroupValidationException moderatorGroupValidationException) {
            return BadRequest(GetInnerMessage(moderatorGroupValidationException));
        }
    }

    private static string GetInnerMessage(Exception exception) =>
        exception.InnerException.Message;
}
