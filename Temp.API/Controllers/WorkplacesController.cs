﻿using Temp.Services.Workplaces;
using Temp.Services.Workplaces.Exceptions;
using Temp.Services.Workplaces.Models.Commands;
using Temp.Services.Workplaces.Models.Queries;

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

    [HttpGet("paged-workplaces")]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(PagedList<GetWorkplacesResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetPagedWorkplaces([FromQuery] GetWorkplacesRequest request) {
        try {
            var response = await _workplaceService.GetPagedWorkplaces(request);
            Response.AddPagination(response.CurrentPage, response.PageSize, response.TotalCount, response.TotalPages);

            return Ok(response);
        } catch (WorkplaceValidationException workplaceValidationException) {
            return BadRequest(GetInnerMessage(workplaceValidationException));
        }
    }

    [HttpGet]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(List<GetWorkplaceResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetWorkplaces() {
        try {
            var response = await _workplaceService.GetWorkplaces();

            return Ok(response);
        } catch (WorkplaceValidationException workplaceValidationException) {
            return BadRequest(GetInnerMessage(workplaceValidationException));
        }
    }

    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(GetWorkplaceResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetWorkplace([FromRoute] int id) {
        try {
            var response = await _workplaceService.GetWorkplace(id);

            return Ok(response);
        } catch (WorkplaceValidationException workplaceValidationException) {
            return BadRequest(GetInnerMessage(workplaceValidationException));
        }
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(CreateWorkplaceResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> CreateWorkplace([FromBody] CreateWorkplaceRequest request) {
        try {
            var response = await _workplaceService.CreateWorkplace(request);

            return Ok(request);
        } catch (WorkplaceValidationException workplaceValidationException) {
            return BadRequest(GetInnerMessage(workplaceValidationException));
        }
    }

    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> UpdateWorkplace([FromBody] UpdateWorkplaceRequest request) {
        try {
            var response = await _workplaceService.UpdateWorkplace(request);

            return NoContent();
        } catch (WorkplaceValidationException workplaceValidationException) {
            return BadRequest(GetInnerMessage(workplaceValidationException));
        }
    }

    [HttpPut("change-status/{id}")]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> UpdateWorkplaceStatus([FromBody] UpdateWorkplaceStatusRequest request) {
        try {
            var response = await _workplaceService.UpdateWorkplaceStatus(request);

            return NoContent();
        } catch (WorkplaceValidationException workplaceValidationException) {
            return BadRequest(GetInnerMessage(workplaceValidationException));
        }
    }

    [HttpGet("workplace-exists")]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
    public async Task<IActionResult> WorkplaceExists([FromQuery] string name) {
        try {
            var response = await _workplaceService.WorkplaceExists(name);

            return Ok(response);
        } catch (WorkplaceValidationException workplaceValidationException) {
            return BadRequest(GetInnerMessage(workplaceValidationException));
        }
    }

    private static string GetInnerMessage(Exception exception) =>
        exception.InnerException.Message;
}
