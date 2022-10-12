using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Temp.Core.Auth.Users;
using Temp.Database;

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

    [HttpPost("register")]
    public async Task<IActionResult> RegisterUser(RegisterUser.Request request) {
        var response = await new RegisterUser(_ctx).Do(request);
        if (response.Status)
            return Ok(response);

        return BadRequest(response);
    }

    [HttpPost("login")]
    public async Task<IActionResult> LoginUser(LoginUser.Request request) {
        if (!ModelState.IsValid)
            return BadRequest(ModelState.Values);

        var response = await new LoginUser(_ctx, _config).Do(request);
        if (response is null)
            return Unauthorized();

        return Ok(response);
    }
}