using Temp.Services.EmploymentStatuses;
using Temp.Services.EmploymentStatuses.Exceptions;
using Temp.Services.EmploymentStatuses.Models.Commands;
using Temp.Services.EmploymentStatuses.Models.Queries;

namespace Temp.API.Controllers;

[Authorize(Roles = "Admin")]
[Route("api/[controller]")]
[ApiController]
public class EmploymentStatusesController : ControllerBase
{
    private readonly IEmploymentStatusService _employmentStatusService;

    public EmploymentStatusesController(IEmploymentStatusService employmentStatusService) {
        _employmentStatusService = employmentStatusService;
    }

    [HttpGet("paged-employmentstatuses")]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(PagedList<GetPagedEmploymentStatusesResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetPagedEmploymentStatuses([FromQuery] GetPagedEmploymentStatusesRequest request) {
        try {
            var response = await _employmentStatusService.GetPagedEmploymentStatuses(request);
            Response.AddPagination(response.CurrentPage, response.PageSize, response.TotalCount, response.TotalPages);

            return Ok(response);
        } catch (EmploymentStatusValidationException employmentStatusValidationException) {
            return BadRequest(GetInnerMessage(employmentStatusValidationException));
        }
    }

    [HttpGet]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(List<GetEmploymentStatusResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetEmploymentStatuses() {
        try {
            var response = await _employmentStatusService.GetEmploymentStatuses();

            return Ok(response);
        } catch (EmploymentStatusValidationException employmentStatusValidationException) {
            return BadRequest(GetInnerMessage(employmentStatusValidationException));
        }
    }

    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(GetEmploymentStatusResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetEmploymentStatus([FromRoute] GetEmploymentStatusRequest request) {
        try {
            var response = await _employmentStatusService.GetEmploymentStatus(request);

            return Ok(response);
        } catch (EmploymentStatusValidationException employmentStatusValidationException) {
            return BadRequest(GetInnerMessage(employmentStatusValidationException));
        }
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(CreateEmploymentStatusResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> Create([FromBody] CreateEmploymentStatusRequest request) {
        try {
            var response = await _employmentStatusService.CreateEmploymentStatus(request);

            return Ok(response);
        } catch (EmploymentStatusValidationException employmentStatusValidationException) {
            return BadRequest(GetInnerMessage(employmentStatusValidationException));
        }
    }

    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> UpdateEmploymentStatus([FromBody] UpdateEmploymentStatusRequest request) {
        try {
            var response = await _employmentStatusService.UpdateEmplymentStatus(request);

            return NoContent();
        } catch (EmploymentStatusValidationException employmentStatusValidationException) {
            return BadRequest(GetInnerMessage(employmentStatusValidationException));
        }
    }

    [HttpPut("change-status/{id}")]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> UpdateEmploymentStatusStatus([FromBody] UpdateEmploymentStatusStatusRequest request) {
        try {
            var response = await _employmentStatusService.UpdateEmploymentStatusStatus(request);

            return NoContent();
        } catch (EmploymentStatusValidationException employmentStatusValidationException) {
            return BadRequest(GetInnerMessage(employmentStatusValidationException));
        }
    }

    [HttpGet("employment-status-exists")]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
    public async Task<IActionResult> EmploymentStatusExists([FromQuery] string name) {
        try {
            var response = await _employmentStatusService.EmploymentStatusExists(name);

            return Ok(response);
        } catch (EmploymentStatusValidationException employmentStatusValidationException) {
            return BadRequest(GetInnerMessage(employmentStatusValidationException));
        }
    }

    private static string GetInnerMessage(Exception exception) =>
        exception.InnerException.Message;
}
