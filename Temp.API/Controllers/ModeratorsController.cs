using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Temp.Application.Auth.Moderators;
using Temp.Database;

namespace Temp.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ModeratorsController : ControllerBase
    {
        private readonly ApplicationDbContext _ctx;
        private readonly IConfiguration _config;

        public ModeratorsController(ApplicationDbContext ctx, IConfiguration config)
        {
            _ctx = ctx;
            _config = config;
        }

        [HttpPost("register")]
        public async Task<IActionResult> RegisterAdmin(RegisterModerator.Request request)
        {
            var response = await new RegisterModerator(_ctx).Do(request);
            if (response.Status)
            {
                return Ok(response);
            }

            return BadRequest(response.Message);
        }
    }
}