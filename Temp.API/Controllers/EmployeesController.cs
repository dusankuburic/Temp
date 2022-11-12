using Temp.Services.Employees;
using Temp.Services.Employees.Exceptions;
using Temp.Services.Employees.Models.Command;
using Temp.Services.Employees.Models.Query;

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
    public async Task<IActionResult> GetEmployees([FromQuery] GetEmployees.Request request) {
        try {
            var response = await _employeeService.GetEmployees(request);
            Response.AddPagination(response.CurrentPage, response.PageSize, response.TotalCount, response.TotalPages);
            return Ok(response);
        } catch (EmployeeValidationException employeeValidationException) {
            return BadRequest(GetInnerMessage(employeeValidationException));
        }
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetEmployee(int id) {
        var response = await _employeeService.GetEmployee(id);
        return Ok(response);
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateEmployee.Request request) {
        try {
            var response = await _employeeService.CreateEmployee(request);
            return NoContent();
        } catch (EmployeeValidationException employeeValidationException) {
            return BadRequest(GetInnerMessage(employeeValidationException));
        }
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateEmployee(int id, UpdateEmployee.Request request) {
        var response = await _employeeService.UpdateEmployee(id, request);
        return response.Status ? NoContent() : BadRequest();
    }

    [HttpPut("change-status/{id}")]
    public async Task<IActionResult> UpdateEmployeeAccountStatus(int id) {
        var response = await _employeeService.UpdateEmployeeAccountStatus(id);
        return response ? NoContent() : BadRequest();
    }

    [HttpPost("assign")]
    public async Task<IActionResult> AssignRole(AssignRole.Request request) {

        //var response = await new AssignRole(_ctx).Do(request);
        //return response.Status ? Ok() : BadRequest(response.Message);
        return Ok();
    }

    [HttpPost("unassign")]
    public async Task<IActionResult> RemoveRole(RemoveEmployeeRole.Request request) {
        //var response = await new RemoveEmployeeRole(_ctx).Do(request);
        //return response.Status ? (IActionResult)Ok() : BadRequest(response.Message);
        return Ok();
    }

    private static string GetInnerMessage(Exception exception) =>
        exception.InnerException.Message;
}

