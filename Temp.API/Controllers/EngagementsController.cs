using Temp.Core.Engagements;
using Temp.Domain.Models.EmploymentStatuses.Exceptions;
using Temp.Domain.Models.Engagements.Exceptions;
using Temp.Services.Employees.Exceptions;
using Temp.Services.Workplaces.Exceptions;

namespace Temp.API.Controllers;

[Authorize(Roles = "Admin, User")]
[Route("api/[controller]")]
[ApiController]
public class EngagementsController : ControllerBase
{
    private readonly ApplicationDbContext _ctx;

    public EngagementsController(ApplicationDbContext ctx) {
        _ctx = ctx;
    }

    //[HttpGet("without")]
    //public async Task<IActionResult> WithoutEngagements([FromQuery] GetEmployeesWithoutEngagement.Request request) {
    //    try {
    //        var response = await new GetEmployeesWithoutEngagement(_ctx).Do(request);
    //        Response.AddPagination(response.CurrentPage, response.PageSize, response.TotalCount, response.TotalPages);
    //        return Ok(response);
    //    } catch (EmployeeValidationEsxception employeeValidationException) {
    //        return BadRequest(GetInnerMessage(employeeValidationException));
    //    } catch (WorkplaceValidationException workplaceValidationException) {
    //        return BadRequest(GetInnerMessage(workplaceValidationException));
    //    } catch (EmploymentStatusValidationException employmentStatusValidationException) {
    //        return BadRequest(GetInnerMessage(employmentStatusValidationException));
    //    }
    //}

    //[HttpGet("with")]
    //public async Task<IActionResult> WithEngagements([FromQuery] GetEmployeesWithEngagement.Request request) {
    //    try {
    //        var response = await new GetEmployeesWithEngagement(_ctx).Do(request);
    //        Response.AddPagination(response.CurrentPage, response.PageSize, response.TotalCount, response.TotalPages);
    //        return Ok(response);
    //    } catch (EmployeeValidationException employeeValidationException) {
    //        return BadRequest(GetInnerMessage(employeeValidationException));
    //    } catch (WorkplaceValidationException workplaceValidationException) {
    //        return BadRequest(GetInnerMessage(workplaceValidationException));
    //    } catch (EmploymentStatusValidationException employmentStatusValidationException) {
    //        return BadRequest(GetInnerMessage(employmentStatusValidationException));

    //    }
    //}

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
            return response.Status ? NoContent() : BadRequest();
        } catch (EngagementValidationException engagementValidationException) {
            return BadRequest(GetInnerMessage(engagementValidationException));
        }
    }

    private static string GetInnerMessage(Exception exception) {
        return exception.InnerException.Message;
    }
}
