using Temp.Services.Organizations;
using Temp.Services.Organizations.Exceptions;
using Temp.Services.Organizations.Models.Commands;

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
    public async Task<IActionResult> Create(CreateOrganization.Request request) {
        try {
            var response = await _organizationService.CreateOrganization(request);
            return response.Status ? NoContent() : BadRequest(response.Message);
        } catch (OrganizationValidationException organizationValidationException) {
            return BadRequest(GetInnerMessage(organizationValidationException));
        }
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetOrganization(int id) {
        try {
            var response = await _organizationService.GetOrganization(id);
            return Ok(response);
        } catch (OrganizationValidationException organizationValidationException) {
            return BadRequest(GetInnerMessage(organizationValidationException));
        }
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateOrganization(int id, UpdateOrganization.Request request) {
        try {
            var response = await _organizationService.UpdateOrganization(id, request);
            return response.Status ? NoContent() : BadRequest(response.Message);
        } catch (OrganizationValidationException organizationValidationException) {
            return BadRequest(GetInnerMessage(organizationValidationException));
        }
    }

    [HttpGet("inner-groups/{id}")]
    public async Task<IActionResult> InnerGroups(int id) {
        try {
            var innerGroups = await _organizationService.GetInnerGroups(id);
            return Ok(innerGroups);
        } catch (OrganizationValidationException organizationValidationException) {
            return BadRequest(GetInnerMessage(organizationValidationException));
        }
    }

    [HttpPut("change-stauts/{id}")]
    public async Task<IActionResult> UpdateOrganizationStatus(int id) {
        var response = await _organizationService.UpdateOrganizationStatus(id);
        return response ? NoContent() : BadRequest();
    }

    private static string GetInnerMessage(Exception exception) {
        return exception.InnerException.Message;
    }
}
