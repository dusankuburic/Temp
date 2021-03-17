using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Temp.Core.Applications;
using Temp.Database;

namespace Temp.API.Controllers
{
    [Authorize(Roles = "Admin")]
    [Route("api/[controller]")]
    [ApiController]
    public class ApplicationsController : ControllerBase
    {
        private readonly ApplicationDbContext _ctx;

        public ApplicationsController(ApplicationDbContext ctx)
        {
            _ctx = ctx;
        }


        [HttpPost]
        public async Task<IActionResult> Create(CreateApplication.Request request)
        {
            var response = await new CreateApplication(_ctx).Do(request);
            if(response.Status)
                return NoContent();

            return BadRequest("Error");
        }



    }
}
