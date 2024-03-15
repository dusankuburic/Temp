using Temp.Domain.Models.ModeratorGroups.Exceptions;
using Temp.Services.Moderators;

namespace Temp.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ModeratorsController : ControllerBase
{
    private readonly ApplicationDbContext _ctx;

    public ModeratorsController(ApplicationDbContext ctx) {
        _ctx = ctx;
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
