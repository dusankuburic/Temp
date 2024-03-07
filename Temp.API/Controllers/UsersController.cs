using Temp.Services.Auth.Users;

namespace Temp.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class UsersController : ControllerBase
{
    private readonly ApplicationDbContext _ctx;
    private readonly IConfiguration _config;

    public UsersController(ApplicationDbContext ctx, IConfiguration config) {
        _ctx = ctx;
        _config = config;
    }

    [Authorize(Roles = "Admin")]
    [HttpPost("register")]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(RegisterUserResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> RegisterUser(RegisterUserRequest request) {
        var response = await new RegisterUser(_ctx).Do(request);
        return response.Status ? Ok(response) : BadRequest(response);
    }

    [HttpPost("login")]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(Response), StatusCodes.Status200OK)]
    public async Task<IActionResult> LoginUser(LoginUserRequest request) {
        if (!ModelState.IsValid)
            return BadRequest(ModelState.Values);

        var response = await new LoginUser(_ctx, _config).Do(request);
        return response is null ? Unauthorized() : Ok(response);
    }
}