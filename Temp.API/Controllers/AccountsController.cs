using Temp.Services.Auth;
using Temp.Services.Auth.Models.Commands;

namespace Temp.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AccountsController : ControllerBase
{
    private readonly IAuthService _authService;

    public AccountsController(IAuthService authService) {
        _authService = authService;
    }

    [Authorize(Roles = "Admin")]
    [HttpPost("register")]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(RegisterResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> Register(RegisterRequest request) {
        var response = await _authService.Register(request);
        return response.Status ? Ok(response) : BadRequest(response.Message);
    }

    [HttpPost("login")]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(LoginResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> Login(LoginRequest request) {
        if (!ModelState.IsValid)
            return BadRequest(ModelState.Values);

        var response = await _authService.Login(request);
        return response is null ? Unauthorized() : Ok(response);
    }

    [Authorize(Roles = "Admin")]
    [HttpPut("unassign/{id}")]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> RemoveRole([FromBody] RemoveEmployeeRoleRequest request) {
        var response = await _authService.RemoveEmployeeRole(request);
        return response.Success ? Ok() : BadRequest();
    }

    [Authorize(Roles = "Admin")]
    [HttpPut("change-status/{id}")]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> UpdateEmployeeAccountStatus([FromRoute] int id) {
        var response = await _authService.UpdateEmployeeAccountStatus(id);
        return response ? NoContent() : BadRequest();
    }
}
