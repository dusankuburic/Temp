using Temp.Domain.Models.ModeratorGroups.Exceptions;
using Temp.Services.Auth.Moderators;

namespace Temp.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ModeratorsController : ControllerBase
{
    private readonly ApplicationDbContext _ctx;
    private readonly IConfiguration _config;

    public ModeratorsController(ApplicationDbContext ctx, IConfiguration config) {
        _ctx = ctx;
        _config = config;
    }

    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(GetModerator.ModeratorViewModel), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetModerator([FromRoute] int id) {
        var response = await new GetModerator(_ctx).Do(id);

        return Ok(response);
    }

    [Authorize(Roles = "Admin")]
    [HttpPost("register")]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> RegisterAdmin(RegisterModeratorRequest request) {
        var response = await new RegisterModerator(_ctx).Do(request);
        if (response.Status)
            return NoContent();

        return BadRequest(response.Message);
    }

    [HttpPost("login")]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(LoginModeratorResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> LoginAdmin(LoginModeratorRequest request) {
        if (!ModelState.IsValid) {
            return BadRequest(ModelState.Values);
        }

        var response = await new LoginModerator(_ctx, _config).Do(request);
        if (response is null)
            return Unauthorized();

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
