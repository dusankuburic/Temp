using Temp.Services.Workplaces;
using Temp.Services.Workplaces.Exceptions;
using Temp.Services.Workplaces.Models.Command;
using Temp.Services.Workplaces.Models.Query;

namespace Temp.API.Controllers;

[Authorize(Roles = "Admin")]
[Route("api/[controller]")]
[ApiController]
public class WorkplacesController : Controller
{
    private readonly IWorkplaceService _workplaceService;

    public WorkplacesController(IWorkplaceService workplaceService) {
        _workplaceService = workplaceService;
    }

    [HttpGet]
    public async Task<IActionResult> GetWorkplaces([FromQuery] GetWorkplacesRequest request) {
        try {
            var response = await _workplaceService.GetPagedWorkplaces(request);
            Response.AddPagination(response.CurrentPage, response.PageSize, response.TotalCount, response.TotalPages);

            return Ok(response);
        } catch (WorkplaceValidationException workplaceValidationException) {
            return BadRequest(GetInnerMessage(workplaceValidationException));
        }
    }

    [HttpPost]
    public async Task<IActionResult> CreateWorkplace(CreateWorkplaceRequest request) {
        try {
            var response = await _workplaceService.CreateWorkplace(request);

            return Ok(request);
        } catch (WorkplaceValidationException workplaceValidationException) {
            return BadRequest(GetInnerMessage(workplaceValidationException));
        }
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetWorkplace(int id) {
        try {
            var response = await _workplaceService.GetWorkplace(id);

            return Ok(response);
        } catch (WorkplaceValidationException workplaceValidationException) {
            return BadRequest(GetInnerMessage(workplaceValidationException));
        }
    }

    [HttpPut]
    public async Task<IActionResult> UpdateWorkplace(UpdateWorkplaceRequest request) {
        try {
            var response = await _workplaceService.UpdateWorkplace(request);

            return Ok(response);
        } catch (WorkplaceValidationException workplaceValidationException) {
            return BadRequest(GetInnerMessage(workplaceValidationException));
        }
    }

    [HttpPut("change-status/")]
    public async Task<IActionResult> UpdateWorkplaceStatus(UpdateWorkplaceStatusRequest request) {
        try {
            var response = await _workplaceService.UpdateWorkplaceStatus(request);

            return Ok(response);
        } catch (WorkplaceValidationException workplaceValidationException) {
            return BadRequest(GetInnerMessage(workplaceValidationException));
        }
    }

    private static string GetInnerMessage(Exception exception) =>
        exception.InnerException.Message;
}
