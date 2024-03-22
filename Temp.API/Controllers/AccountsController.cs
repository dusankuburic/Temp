using Temp.Services.Auth;
using Temp.Services.Auth.Exceptions;
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

    [HttpPost("login")]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(LoginAppUserResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> Login([FromBody] LoginAppUserRequest request) {
        try {
            var response = await _authService.Login(request);
            return Ok(response);
        } catch (UserValidationException) {
            return Unauthorized();
        }
    }

    [Authorize(Roles = "Admin")]
    [HttpPost("register")]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> Register([FromBody] RegisterAppUserRequest request) {
        try {
            await _authService.Register(request);
            return Ok();
        } catch (UserValidationException userValidationException) {
            return BadRequest(GetInnerMessage(userValidationException));
        }
    }

    [Authorize(Roles = "Admin,User,Moderator")]
    [HttpPost("logout")]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> Logout() {
        var response = await _authService.Logout();
        return response ? Ok() : BadRequest();
    }

    [Authorize(Roles = "Admin")]
    [HttpPut("unassign/{id}")]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> RemoveRole([FromBody] RemoveEmployeeRoleRequest request) {
        try {
            var response = await _authService.RemoveEmployeeRole(request);
            return Ok();
        } catch (UserValidationException userValidationException) {
            return BadRequest(GetInnerMessage(userValidationException));
        }
    }

    [Authorize(Roles = "Admin")]
    [HttpPut("change-status/{id}")]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> UpdateEmployeeAccountStatus([FromRoute] int id) {
        try {
            var response = await _authService.UpdateEmployeeAccountStatus(id);
            return NoContent();
        } catch (UserValidationException userValidationException) {
            return BadRequest(GetInnerMessage(userValidationException));
        }
    }

    [Authorize(Roles = "Admin")]
    [HttpGet("username-exists")]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> UsernameExists([FromQuery] string username) {
        try {
            var response = await _authService.CheckUsernameExists(username);

            return Ok(response);
        } catch (UserValidationException userValidationException) {
            return BadRequest(GetInnerMessage(userValidationException));
        }
    }

    [Authorize(Roles = "Admin")]
    [HttpGet("employee-username/{id}")]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetEmployeeUsername([FromRoute] int id) {
        var username = await _authService.GetEmployeeUsername(id);

        return username != null ? Ok(new { username }) : BadRequest();
    }

    private string GetInnerMessage(Exception exception) =>
        exception.InnerException.Message;
}
