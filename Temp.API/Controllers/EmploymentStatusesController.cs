using Temp.Core.EmploymentStatuses;
using Temp.Domain.Models.EmploymentStatuses.Exceptions;

namespace Temp.API.Controllers;

[Authorize(Roles = "Admin")]
[Route("api/[controller]")]
[ApiController]
public class EmploymentStatusesController : ControllerBase
{
    private readonly ApplicationDbContext _ctx;

    public EmploymentStatusesController(ApplicationDbContext ctx) {
        _ctx = ctx;
    }

    [HttpGet]
    public async Task<IActionResult> GetEmploymentStatuses([FromQuery] GetEmploymentStatuses.Request request) {
        try {
            var response = await new GetEmploymentStatuses(_ctx).Do(request);
            Response.AddPagination(response.CurrentPage, response.PageSize, response.TotalCount, response.TotalPages);

            return Ok(response);
        } catch (EmploymentStatusValidationException employmentStatusValidationException) {
            return BadRequest(GetInnerMessage(employmentStatusValidationException));
        }
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateEmploymentStatus.Request request) {
        try {
            var response = await new CreateEmploymentStatus(_ctx).Do(request);
            if (response.Status)
                return NoContent();

            return BadRequest(response.Message);
        } catch (EmploymentStatusValidationException employmentStatusValidationException) {
            return BadRequest(GetInnerMessage(employmentStatusValidationException));
        }
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetEmploymentStatus(int id) {
        try {
            var response = await new GetEmploymentStatus(_ctx).Do(id);
            return Ok(response);
        } catch (EmploymentStatusValidationException employmentStatusValidationException) {
            return BadRequest(GetInnerMessage(employmentStatusValidationException));
        }
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateEmploymentStatus(int id, UpdateEmploymentStatus.Request request) {
        try {
            var response = await new UpdateEmploymentStatus(_ctx).Do(id, request);
            if (response.Status)
                NoContent();

            return BadRequest(response.Message);
        } catch (EmploymentStatusValidationException employmentStatusValidationException) {
            return BadRequest(GetInnerMessage(employmentStatusValidationException));
        }
    }

    [HttpPut("change-status/{id}")]
    public async Task<IActionResult> UpdateEmploymentStatusStatus(int id) {
        var response = await new UpdateEmploymentStatusStatus(_ctx).Do(id);
        if (response)
            return NoContent();

        return BadRequest();
    }

    private static string GetInnerMessage(Exception exception) =>
        exception.InnerException.Message;
}
