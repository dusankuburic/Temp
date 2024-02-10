using Temp.Domain.Models.EmploymentStatuses.Exceptions;
using Temp.Domain.Models.Engagements.Exceptions;
using Temp.Services.Employees;
using Temp.Services.Employees.Exceptions;
using Temp.Services.Employees.Models.Query;
using Temp.Services.Engagements;
using Temp.Services.Engagements.Models.Commands;
using Temp.Services.Engagements.Models.Queries;
using Temp.Services.Workplaces.Exceptions;

namespace Temp.API.Controllers;

[Authorize(Roles = "Admin, User")]
[Route("api/[controller]")]
[ApiController]
public class EngagementsController : ControllerBase
{
    private readonly IEngagementService _engagementService;
    private readonly IEmployeeService _employeeService;

    public EngagementsController(
        IEngagementService engagementService,
        IEmployeeService employeeService) {
        _engagementService = engagementService;
        _employeeService = employeeService;
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateEngagementRequest request) {
        try {
            var response = await _engagementService.CreateEngagement(request);

            return Ok(response);
        } catch (EngagementValidationException engagementValidationException) {
            return BadRequest(GetInnerMessage(engagementValidationException));
        }
    }

    [Authorize(Roles = "User")]
    [HttpGet("user/{id}")]
    public async Task<IActionResult> GetUserEmployeeEngagments([FromQuery] GetUserEmployeeEngagementsRequest request) {
        try {
            var response = await _engagementService.GetUserEmployeeEngagements(request);

            return Ok(response);
        } catch (EngagementValidationException engagementValidationException) {
            return BadRequest(GetInnerMessage(engagementValidationException));
        }
    }

    [HttpGet("without")]
    public async Task<IActionResult> WithoutEngagements([FromQuery] GetEmployeesWithoutEngagement.Request request) {
        try {
            var response = await _employeeService.GetEmployeesWithoutEngagement(request);
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
            var response = await _employeeService.GetEmployeesWithEngagement(request);
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

    [HttpGet("employee/{id}")]
    public async Task<IActionResult> GetEngagementForEmployee([FromQuery] GetEngagementsForEmployeeRequest request) {
        var response = await _engagementService.GetEngagementForEmployee(request);

        return Ok(response);
    }

    private static string GetInnerMessage(Exception exception) {
        return exception.InnerException.Message;
    }
}
