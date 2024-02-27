using Temp.Services.Employees;
using Temp.Services.Employees.Exceptions;
using Temp.Services.Employees.Models.Commands;
using Temp.Services.Employees.Models.Queries;

namespace Temp.API.Controllers;

[Authorize(Roles = "Admin")]
[Route("api/[controller]")]
[ApiController]
public class EmployeesController : ControllerBase
{
    private readonly IEmployeeService _employeeService;

    public EmployeesController(IEmployeeService employeeService) {
        _employeeService = employeeService;
    }

    [HttpGet]
    public async Task<IActionResult> GetEmployees([FromQuery] GetEmployeesRequest request) {
        try {
            var response = await _employeeService.GetEmployees(request);
            Response.AddPagination(response.CurrentPage, response.PageSize, response.TotalCount, response.TotalPages);
            return Ok(response);
        } catch (EmployeeValidationException employeeValidationException) {
            return BadRequest(GetInnerMessage(employeeValidationException));
        }
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetEmployee([FromRoute] int id) {
        try {
            var response = await _employeeService.GetEmployee(id);

            return Ok(response);
        } catch (EmployeeValidationException employeeValidationException) {
            return BadRequest(GetInnerMessage(employeeValidationException));
        }
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateEmployeeRequest request) {
        try {
            var response = await _employeeService.CreateEmployee(request);

            return Ok(response);
        } catch (EmployeeValidationException employeeValidationException) {
            return BadRequest(GetInnerMessage(employeeValidationException));
        }
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateEmployee([FromBody] UpdateEmployeeRequest request) {
        try {
            var response = await _employeeService.UpdateEmployee(request);

            return NoContent();
        } catch (EmployeeValidationException employeeValidationException) {
            return BadRequest(GetInnerMessage(employeeValidationException));
        }
    }

    [HttpPut("change-status/{id}")]
    public async Task<IActionResult> UpdateEmployeeAccountStatus([FromBody] int id) {
        var response = await _employeeService.UpdateEmployeeAccountStatus(id);
        return response ? NoContent() : BadRequest();
    }

    [HttpPut("assign/{id}")]
    public async Task<IActionResult> AssignRole([FromBody] AssignRoleRequest request) {

        //var response = await new AssignRole(_ctx).Do(request);
        //return response.Status ? Ok() : BadRequest(response.Message);
        return Ok();
    }

    [HttpPut("unassign/{id}")]
    public async Task<IActionResult> RemoveRole([FromBody] RemoveEmployeeRoleRequest request) {
        //var response = await new RemoveEmployeeRole(_ctx).Do(request);
        //return response.Status ? (IActionResult)Ok() : BadRequest(response.Message);
        return Ok();
    }

    private static string GetInnerMessage(Exception exception) =>
        exception.InnerException.Message;
}

