using Temp.Core.Auth.Admins;

namespace Temp.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AdminsController : ControllerBase
{
    private readonly ApplicationDbContext _ctx;
    private readonly IConfiguration _config;

    public AdminsController(ApplicationDbContext ctx, IConfiguration config) {
        _ctx = ctx;
        _config = config;
    }

    [HttpPost("register")]
    public async Task<IActionResult> RegisterAdmin(RegisterAdminRequest request) {
        var response = await new RegisterAdmin(_ctx).Do(request);
        return response.Status ? Ok(response) : BadRequest(response.Message);
    }

    [HttpPost("login")]
    public async Task<IActionResult> LoginAdmin(LoginAdminRequest request) {
        if (!ModelState.IsValid)
            return BadRequest(ModelState.Values);

        var response = await new LoginAdmin(_ctx, _config).Do(request);
        return response is null ? Unauthorized() : Ok(response);
    }
}
