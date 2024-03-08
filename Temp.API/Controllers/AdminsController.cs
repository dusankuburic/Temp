using Temp.Services.Auth;

namespace Temp.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AdminsController : ControllerBase
{
    private readonly IAuthService _authService;

    public AdminsController(IAuthService authService) {
        _authService = authService;
    }

    [Authorize(Roles = "Admin")]
    [HttpPost("register")]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(RegisterAdminResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> RegisterAdmin(RegisterAdminRequest request) {
        var response = await _authService.RegisterAdmin(request);
        return response.Status ? Ok(response) : BadRequest(response.Message);
    }

    [HttpPost("login")]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(LoginAResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> LoginAdmin(LoginAdminRequest request) {
        if (!ModelState.IsValid)
            return BadRequest(ModelState.Values);

        var response = await _authService.LoginAdmin(request);
        return response is null ? Unauthorized() : Ok(response);
    }
}
