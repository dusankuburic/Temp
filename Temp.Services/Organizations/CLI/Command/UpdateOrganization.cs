using Temp.Database;
using Temp.Domain.Models;

namespace Temp.Services.Organizations.CLI.Command;

public class UpdateOrganization
{
    private readonly ApplicationDbContext _ctx;

    public UpdateOrganization(ApplicationDbContext ctx) {
        _ctx = ctx;
    }

    public async Task<Response> Do(Organization organization) {

        await _ctx.SaveChangesAsync();

        return new Response {
            Id = organization.Id,
            Name = organization.Name,
            Message = "Success",
            Status = true
        };
    }

    public class Request
    {
        [MinLength(2)]
        [MaxLength(50)]
        public string Name { get; set; }
    }

    public class Response
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Message { get; set; }
        public bool Status { get; set; }
    }
}

