using Temp.Services.Auth;
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
    private readonly IAuthService _authService;

    public EmployeesController(
        IEmployeeService employeeService,
        IAuthService authService) {
        _employeeService = employeeService;
        _authService = authService;
    }

    [HttpGet]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(PagedList<GetEmployeesResponse>), StatusCodes.Status200OK)]
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
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(GetEmployeesResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetEmployee([FromRoute] int id) {
        try {
            var response = await _employeeService.GetEmployee(id);

            return Ok(response);
        } catch (EmployeeValidationException employeeValidationException) {
            return BadRequest(GetInnerMessage(employeeValidationException));
        }
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(CreateEmployeeResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> Create([FromBody] CreateEmployeeRequest request) {
        try {
            var response = await _employeeService.CreateEmployee(request);

            return Ok(response);
        } catch (EmployeeValidationException employeeValidationException) {
            return BadRequest(GetInnerMessage(employeeValidationException));
        }
    }

    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> UpdateEmployee([FromBody] UpdateEmployeeRequest request) {
        try {
            var response = await _employeeService.UpdateEmployee(request);

            return NoContent();
        } catch (EmployeeValidationException employeeValidationException) {
            return BadRequest(GetInnerMessage(employeeValidationException));
        }
    }

    [HttpPost("assign/{id}")]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> AssignRole([FromBody] AssignRoleRequest request) {
        var response = await _authService.AssignRole(request);
        return response.Status ? Ok() : BadRequest(response.Message);
    }

    private static string GetInnerMessage(Exception exception) =>
        exception.InnerException.Message;
}

