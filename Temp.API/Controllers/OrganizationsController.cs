using Temp.Services.Organizations;
using Temp.Services.Organizations.CLI.Command;
using Temp.Services.Organizations.Exceptions;

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
            if (response.Status)
                return NoContent();

            return BadRequest(response.Message);
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
            if (response.Status)
                return NoContent();

            return BadRequest(response.Message);
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
        if (response)
            return NoContent();

        return BadRequest();
    }

    private static string GetInnerMessage(Exception exception) =>
        exception.InnerException.Message;
}
