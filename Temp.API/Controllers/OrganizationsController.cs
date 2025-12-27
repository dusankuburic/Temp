using Temp.Services.Organizations;
using Temp.Services.Organizations.Exceptions;
using Temp.Services.Organizations.Models.Commands;
using Temp.Services.Organizations.Models.Queries;

namespace Temp.API.Controllers;

[Authorize(Roles = "Admin")]
[Route("api/[controller]")]
[ApiController]
public class OrganizationsController : ControllerBase
{
    private readonly IOrganizationService _organizationService;

    public OrganizationsController(IOrganizationService organizationService) {
        _organizationService = organizationService;
    }

    [HttpGet("paged-organizations")]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(PagedList<GetOrganizationResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetPagedOrganizations([FromQuery] GetOrganizationsRequest request) {
        var response = await _organizationService.GetPagedOrganizations(request);
        Response.AddPagination(response.CurrentPage, response.PageSize, response.TotalCount, response.TotalPages);

        return Ok(response);
    }

    [HttpGet]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(IEnumerable<GetOrganizationResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetOrganizations() {
        var response = await _organizationService.GetOrganizations();

        return Ok(response);
    }

    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(GetOrganizationResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetOrganization([FromRoute] GetOrganizationRequest request) {
        var response = await _organizationService.GetOrganization(request);

        return Ok(response);
    }

    [HttpGet("paged-inner-groups")]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(GetPagedInnerGroupsResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetPagedInnerGroups([FromQuery] GetOrganizationInnerGroupsRequest request) {
        var response = await _organizationService.GetPagedInnerGroups(request);
        Response.AddPagination(response.Groups.CurrentPage, response.Groups.PageSize, response.Groups.TotalCount, response.Groups.TotalPages);

        return Ok(response);
    }

    [HttpGet("inner-groups/{id}")]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(List<InnerGroup>), StatusCodes.Status200OK)]
    public async Task<IActionResult> InnerGroups([FromRoute] int id) {
        var response = await _organizationService.GetInnerGroups(id);

        return Ok(response);
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(CreateOrganizationResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> Create([FromBody] CreateOrganizationRequest request) {
        var response = await _organizationService.CreateOrganization(request);

        return Ok(response);
    }

    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> UpdateOrganization([FromBody] UpdateOrganizationRequest request) {
        var response = await _organizationService.UpdateOrganization(request);

        return NoContent();
    }

    [HttpPut("change-status/{id}")]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> UpdateOrganizationStatus([FromBody] UpdateOrganizationStatusRequest request) {
        var response = await _organizationService.UpdateOrganizationStatus(request);

        return NoContent();
    }

    [HttpGet("organization-exists")]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> OrganizationExists([FromQuery] string name) {
        var response = await _organizationService.OrganizationExists(name);

        return Ok(response);
    }

}