using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Temp.Application.Employees;
using Temp.Application.Engagements;
using Temp.Database;
using Temp.Domain.Models.Employees.Exceptions;
using Temp.Domain.Models.EmploymentStatuses.Exceptions;
using Temp.Domain.Models.Workplaces.Exceptions;
using Temp.Domain.Models.Engagements.Exceptions;

namespace Temp.API.Controllers
{
    [Authorize(Roles = "Admin")]
    [Route("api/[controller]")]
    [ApiController]
    public class EngagementsController : ControllerBase
    {
        private readonly ApplicationDbContext _ctx;

        public EngagementsController(ApplicationDbContext ctx)
        {
            _ctx = ctx;
        }
        

        [HttpGet("without")]
        public IActionResult WithoutEngagements()
        {
            try
            {
                var response = new GetEmployeesWithoutEngagement(_ctx).Do();
                return Ok(response);
            }
            catch(EmployeeValidationException employeeValidationException)
            {
                return BadRequest(GetInnerMessage(employeeValidationException));
           
            }
            catch(WorkplaceValidationException workplaceValidationException)
            {
                 return BadRequest(GetInnerMessage(workplaceValidationException));
                
            }
            catch(EmploymentStatusValidationException employmentStatusValidationException)
            {
                return BadRequest(GetInnerMessage(employmentStatusValidationException));
            }
        }

        [HttpGet("with")]
        public IActionResult WithEngagements()
        {
            try
            {
                var response = new GetEmployeesWithEngagement(_ctx).Do();
                return Ok(response);
            }
            catch(EmployeeValidationException employeeValidationException)
            {
                return BadRequest(GetInnerMessage(employeeValidationException));

            }
            catch(WorkplaceValidationException workplaceValidationException)
            {
                return BadRequest(GetInnerMessage(workplaceValidationException));
               
            }
            catch(EmploymentStatusValidationException employmentStatusValidationException)
            {
                return BadRequest(GetInnerMessage(employmentStatusValidationException));
                
            }
        }

        
        [HttpGet("employee/{id}")]
        public ActionResult GetEngagementForEmployee(int id)
        {
            try
            {
                var response = new GetCreateEngagementViewModel(_ctx).Do(id);
                return Ok(response);
            }
            catch(EmployeeValidationException employeeValidationException)
            {
                return BadRequest(GetInnerMessage(employeeValidationException));             
            }
            catch(WorkplaceValidationException workplaceValidationException)
            {
                return BadRequest(GetInnerMessage(workplaceValidationException));                    
            }
            catch(EmploymentStatusValidationException employmentStatusValidationException)
            {
                return BadRequest(GetInnerMessage(employmentStatusValidationException));             
            }
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateEngagement.Request request)
        {
            try
            {
                var response = await new CreateEngagement(_ctx).Do(request);
                if(response.Status)
                {
                    return Ok(response.Message);
                }

                return BadRequest();
            }
            catch(EngagementValidationException engagementValidationException)
            {
                 return BadRequest(GetInnerMessage(engagementValidationException));
            }
        }
        
        
        private static string GetInnerMessage(Exception exception) =>
            exception.InnerException.Message;
    }
}
