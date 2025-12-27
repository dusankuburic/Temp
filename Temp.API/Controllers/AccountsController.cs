using Microsoft.AspNetCore.RateLimiting;
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
    [EnableRateLimiting("auth")]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(LoginAppUserResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> Login([FromBody] LoginAppUserRequest request) {
        var response = await _authService.Login(request);
        return Ok(response);
    }

    [Authorize(Roles = "Admin")]
    [HttpPost("register")]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> Register([FromBody] RegisterAppUserRequest request) {
        await _authService.Register(request);
        return Ok();
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
        var response = await _authService.RemoveEmployeeRole(request);
        return Ok();
    }

    [Authorize(Roles = "Admin")]
    [HttpPut("change-status/{id}")]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> UpdateEmployeeAccountStatus([FromRoute] int id) {
        var response = await _authService.UpdateEmployeeAccountStatus(id);
        return NoContent();
    }

    [Authorize(Roles = "Admin")]
    [HttpGet("username-exists")]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> UsernameExists([FromQuery] string username) {
        var response = await _authService.CheckUsernameExists(username);

        return Ok(response);
    }

    [Authorize(Roles = "Admin")]
    [HttpGet("employee-username/{id}")]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetEmployeeUsername([FromRoute] int id) {
        var username = await _authService.GetEmployeeUsername(id);

        return !string.IsNullOrEmpty(username) ? Ok(new { username }) : BadRequest();
    }

}