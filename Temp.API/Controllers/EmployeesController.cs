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
        var response = await _employeeService.GetEmployee(id);
        return Ok(response);
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateEmployeeRequest request) {
        try {
            var response = await _employeeService.CreateEmployee(request);

            return Ok(response);
        } catch (EmployeeValidationException employeeValidationException) {
            return BadRequest(GetInnerMessage(employeeValidationException));
        }
    }

    [HttpPut]
    public async Task<IActionResult> UpdateEmployee(UpdateEmployeeRequest request) {
        var response = await _employeeService.UpdateEmployee(request);

        return Ok(response);
    }

    [HttpPut("change-status")]
    public async Task<IActionResult> UpdateEmployeeAccountStatus(int id) {
        var response = await _employeeService.UpdateEmployeeAccountStatus(id);
        return response ? NoContent() : BadRequest();
    }

    [HttpPost("assign")]
    public async Task<IActionResult> AssignRole(AssignRoleRequest request) {

        //var response = await new AssignRole(_ctx).Do(request);
        //return response.Status ? Ok() : BadRequest(response.Message);
        return Ok();
    }

    [HttpPost("unassign")]
    public async Task<IActionResult> RemoveRole(RemoveEmployeeRoleRequest request) {
        //var response = await new RemoveEmployeeRole(_ctx).Do(request);
        //return response.Status ? (IActionResult)Ok() : BadRequest(response.Message);
        return Ok();
    }

    private static string GetInnerMessage(Exception exception) =>
        exception.InnerException.Message;
}

