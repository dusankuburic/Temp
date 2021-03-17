using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Temp.API.Helpers;
using Temp.Core.Workplaces;
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
        
        [HttpGet]
        public async Task<IActionResult> GetWorkplaces([FromQuery]GetWorkplaces.Request request)
        {
            try
            {
                var response = await new GetWorkplaces(_ctx).Do(request);
                Response.AddPagination(response.CurrentPage, response.PageSize, response.TotalCount, response.TotalPages);

                return Ok(response);
            }
            catch (WorkplaceValidationException workplaceValidationException)
            {
                return BadRequest(GetInnerMessage(workplaceValidationException));
            }
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateWorkplace.Request request)
        {
            try
            {
                var response = await new CreateWorkplace(_ctx).Do(request);
                if (response.Status)
                    return NoContent();
                
                return BadRequest(response.Message);
            }
            catch (WorkplaceValidationException workplaceValidationException)
            {
                return BadRequest(GetInnerMessage(workplaceValidationException));
            }
        }
        

        [HttpGet("{id}")]
        public async Task<IActionResult> GetWorkplace(int id)
        {
            try
            {
                var response = await new GetWorkplace(_ctx).Do(id);
                return Ok(response);
            }
            catch (WorkplaceValidationException workplaceValidationException)
            {
                return BadRequest(GetInnerMessage(workplaceValidationException));
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateWorkplace(int id, UpdateWorkplace.Request request)
        {
            try
            {
                var response = await new UpdateWorkplace(_ctx).Do(id, request);
                if (response.Status)
                    return NoContent();

                return BadRequest(response.Message);
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