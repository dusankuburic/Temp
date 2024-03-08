using Temp.Services.Auth;

namespace Temp.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class UsersController : ControllerBase
{
    private readonly IAuthService _authService;

    public UsersController(IAuthService authService) {
        _authService = authService;
    }

    [Authorize(Roles = "Admin")]
    [HttpPost("register")]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(RegisterUserResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> RegisterUser(RegisterUserRequest request) {
        var response = await _authService.RegisterUser(request);
        return response.Status ? Ok(response) : BadRequest(response);
    }

    [HttpPost("login")]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(Response), StatusCodes.Status200OK)]
    public async Task<IActionResult> LoginUser(LoginUserRequest request) {
        if (!ModelState.IsValid)
            return BadRequest(ModelState.Values);

        var response = await _authService.LoginUser(request);
        return response is null ? Unauthorized() : Ok(response);
    }
}