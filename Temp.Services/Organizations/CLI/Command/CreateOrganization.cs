using Temp.Database;
using Temp.Domain.Models;

namespace Temp.Services.Organizations.CLI.Command;

public class CreateOrganization
{
    private readonly ApplicationDbContext _ctx;

    public CreateOrganization(ApplicationDbContext ctx) {
        _ctx = ctx;
    }

    public async Task<Response> Do(Organization organization) {

        _ctx.Organizations.Add(organization);
        await _ctx.SaveChangesAsync();

        return new Response {
            Message = $"Success {organization.Name} is added",
            Status = true
        };
    }

    public class Request
    {
        [Required]
        [MinLength(2)]
        [MaxLength(50)]
        public string Name { get; set; }
    }

    public class Response
    {
        public string Message { get; set; }
        public bool Status { get; set; }
    }
}

