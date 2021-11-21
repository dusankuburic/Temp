using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Temp.API.Helpers;
using Temp.Core.Employees;
using Temp.Core.Engagements;
using Temp.Database;
using Temp.Domain.Models.Employees.Exceptions;
using Temp.Domain.Models.EmploymentStatuses.Exceptions;
using Temp.Domain.Models.Engagements.Exceptions;
using Temp.Domain.Models.Workplaces.Exceptions;

namespace Temp.API.Controllers
{
    [Authorize(Roles = "Admin, User")]
    [Route("api/[controller]")]
    [ApiController]
    public class EngagementsController : ControllerBase
    {
        private readonly ApplicationDbContext _ctx;

        public EngagementsController(ApplicationDbContext ctx) {
            _ctx = ctx;
        }

        [HttpGet("without")]
        public async Task<IActionResult> WithoutEngagements([FromQuery] GetEmployeesWithoutEngagement.Request request) {
            try {
                var response = await new GetEmployeesWithoutEngagement(_ctx).Do(request);
                Response.AddPagination(response.CurrentPage, response.PageSize, response.TotalCount, response.TotalPages);
                return Ok(response);
            } catch (EmployeeValidationException employeeValidationException) {
                return BadRequest(GetInnerMessage(employeeValidationException));
            } catch (WorkplaceValidationException workplaceValidationException) {
                return BadRequest(GetInnerMessage(workplaceValidationException));
            } catch (EmploymentStatusValidationException employmentStatusValidationException) {
                return BadRequest(GetInnerMessage(employmentStatusValidationException));
            }
        }

        [HttpGet("with")]
        public async Task<IActionResult> WithEngagements([FromQuery] GetEmployeesWithEngagement.Request request) {
            try {
                var response = await new GetEmployeesWithEngagement(_ctx).Do(request);
                Response.AddPagination(response.CurrentPage, response.PageSize, response.TotalCount, response.TotalPages);
                return Ok(response);
            } catch (EmployeeValidationException employeeValidationException) {
                return BadRequest(GetInnerMessage(employeeValidationException));
            } catch (WorkplaceValidationException workplaceValidationException) {
                return BadRequest(GetInnerMessage(workplaceValidationException));
            } catch (EmploymentStatusValidationException employmentStatusValidationException) {
                return BadRequest(GetInnerMessage(employmentStatusValidationException));

            }
        }

        [Authorize(Roles = "User")]
        [HttpGet("user/{id}")]
        public async Task<IActionResult> GetUserEmployeeEngagments(int id) {
            try {
                var response = await new GetUserEmployeeEngagements(_ctx).Do(id);
                return Ok(response);
            } catch (EngagementValidationException engagementValidationException) {
                return BadRequest(GetInnerMessage(engagementValidationException));
            }
        }

        [HttpGet("employee/{id}")]
        public async Task<IActionResult> GetEngagementForEmployee(int id) {
            try {
                var response = await new GetCreateEngagementViewModel(_ctx).Do(id);
                return Ok(response);
            } catch (EmployeeValidationException employeeValidationException) {
                return BadRequest(GetInnerMessage(employeeValidationException));
            } catch (WorkplaceValidationException workplaceValidationException) {
                return BadRequest(GetInnerMessage(workplaceValidationException));
            } catch (EmploymentStatusValidationException employmentStatusValidationException) {
                return BadRequest(GetInnerMessage(employmentStatusValidationException));
            }
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateEngagement.Request request) {
            try {
                var response = await new CreateEngagement(_ctx).Do(request);
                if (response.Status)
                    return NoContent();

                return BadRequest();
            } catch (EngagementValidationException engagementValidationException) {
                return BadRequest(GetInnerMessage(engagementValidationException));
            }
        }

        private static string GetInnerMessage(Exception exception) =>
            exception.InnerException.Message;
    }
}