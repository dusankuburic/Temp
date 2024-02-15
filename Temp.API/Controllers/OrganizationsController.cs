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

    [HttpGet]
    public async Task<IActionResult> GetOrganizations() {
        try {
            var response = await _organizationService.GetOrganizations();

            return Ok(response);
        } catch (OrganizationValidationException organizationValidationException) {
            return BadRequest(GetInnerMessage(organizationValidationException));
        }
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateOrganizationRequest request) {
        try {
            var response = await _organizationService.CreateOrganization(request);

            return Ok(response);
        } catch (OrganizationValidationException organizationValidationException) {
            return BadRequest(GetInnerMessage(organizationValidationException));
        }
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetOrganization([FromRoute] GetOrganizationRequest request) {
        try {
            var response = await _organizationService.GetOrganization(request);

            return Ok(response);
        } catch (OrganizationValidationException organizationValidationException) {
            return BadRequest(GetInnerMessage(organizationValidationException));
        }
    }

    [HttpPut]
    public async Task<IActionResult> UpdateOrganization(UpdateOrganizationRequest request) {
        try {
            var response = await _organizationService.UpdateOrganization(request);

            return Ok(response);
        } catch (OrganizationValidationException organizationValidationException) {
            return BadRequest(GetInnerMessage(organizationValidationException));
        }
    }

    [HttpGet("inner-groups/{id}")]
    public async Task<IActionResult> InnerGroups([FromRoute] int id) {
        try {
            var innerGroups = await _organizationService.GetInnerGroups(id);
            return Ok(innerGroups);
        } catch (OrganizationValidationException organizationValidationException) {
            return BadRequest(GetInnerMessage(organizationValidationException));
        }
    }

    [HttpPut("change-status")]
    public async Task<IActionResult> UpdateOrganizationStatus(UpdateOrganizationStatusRequest request) {
        var response = await _organizationService.UpdateOrganizationStatus(request);

        return Ok(response);
    }

    private static string GetInnerMessage(Exception exception) {
        return exception.InnerException.Message;
    }
}
