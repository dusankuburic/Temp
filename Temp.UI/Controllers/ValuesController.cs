using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Temp.Application.Workplaces;
using Temp.Database;

namespace Temp.UI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ValuesController : ControllerBase
    {
        public readonly ApplicationDbContext _ctx;

        public ValuesController(ApplicationDbContext ctx)
        {
            _ctx = ctx;
        }

        public IActionResult ListAll() => Ok(new GetWorkplaces(_ctx).Do());
    }
}
