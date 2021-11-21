using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Temp.Core.Auth.Admins;
using Temp.Database;

namespace Temp.API.Controllers
{

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
        public async Task<IActionResult> RegisterAdmin(RegisterAdmin.Request request) {
            var response = await new RegisterAdmin(_ctx).Do(request);
            if (response.Status)
                return Ok(response);

            return BadRequest(response.Message);
        }

        [HttpPost("login")]
        public async Task<IActionResult> LoginAdmin(LoginAdmin.Request request) {
            if (!ModelState.IsValid)
                return BadRequest(ModelState.Values);

            var response = await new LoginAdmin(_ctx, _config).Do(request);
            if (response is null)
                return Unauthorized();

            return Ok(response);
        }
    }
}