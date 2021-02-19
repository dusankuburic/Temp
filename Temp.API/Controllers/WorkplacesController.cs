using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Temp.Application.Workplaces;
using Temp.Database;
using Temp.Domain.Models.Workplaces.Exceptions;

namespace Temp.API.Controllers
{

    [Authorize(Roles = "Admin")]
    [Route("api/[controller]")]
    [ApiController]
    public class WorkplacesController : Controller
    {
        private readonly ApplicationDbContext _ctx;

        public WorkplacesController(ApplicationDbContext ctx)
        {
            _ctx = ctx;
        }
        
        [HttpPost]
        public async Task<IActionResult> Create(CreateWorkplace.Request request)
        {
            try
            {
                var response = await new CreateWorkplace(_ctx).Do(request);
                if(response.Status)
                {
                    return Ok(response);
                }
                else
                {
                    return BadRequest(response.Message);
                }
            }
            catch(WorkplaceValidationException workplaceValidationException)
            {
                return BadRequest(GetInnerMessage(workplaceValidationException));
            }
        }

        [HttpGet]
        public ActionResult<IEquatable<GetWorkplaces.WorkplacesViewModel>> GetWorkplaces()
        {
            try
            {
                var response = new GetWorkplaces(_ctx).Do();
                return Ok(response);

            }
            catch (WorkplaceValidationException workplaceValidationException)
            {
                return BadRequest(GetInnerMessage(workplaceValidationException));
            }
        }

        [HttpGet("{id}")]
        public IActionResult GetWorkplace(int id)
        {
            try
            {
                var response = new GetWorkplace(_ctx).Do(id);
                return Ok(response);
            }
            catch(WorkplaceValidationException workplaceValidationException)
            {
                return BadRequest(GetInnerMessage(workplaceValidationException));
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateWorkplace(UpdateWorkplace.Request request)
        {
            try
            {                
                var response = await new UpdateWorkplace(_ctx).Do(request);
                if(response.Status)
                {
                     return Ok(response.Message);
                }
                else
                {
                    return BadRequest(response.Message);
                }
            }
            catch (WorkplaceValidationException workplaceValidationException)
            { 
                return BadRequest(GetInnerMessage(workplaceValidationException));
            }
        }
  

        private static string GetInnerMessage(Exception exception) =>
            exception.InnerException.Message;
    }
        
}
