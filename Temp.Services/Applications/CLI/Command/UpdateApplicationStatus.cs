using Temp.Database;
using Temp.Domain.Models.Applications;

namespace Temp.Services.Applications.CLI.Command;

public class UpdateApplicationStatus
{
    private readonly ApplicationDbContext _ctx;


    public UpdateApplicationStatus(ApplicationDbContext ctx) {
        _ctx = ctx;
    }

    public async Task<Response> Do(Application application, Request request) {

        application.ModeratorId = request.ModeratorId;
        application.Status = true;
        application.StatusUpdatedAt = DateTime.Now;

        await _ctx.SaveChangesAsync();

        return new Response {
            Id = application.Id,
            Status = true
        };
    }

    public class Request
    {
        public int ModeratorId { get; set; }
    }

    public class Response
    {
        public int Id { get; set; }
        public bool Status { get; set; }
    }
}

