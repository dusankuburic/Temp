using Temp.Core.Workplaces.Service;
using Temp.Database;
using Temp.Domain.Models;

namespace Temp.Core.Workplaces;

public class CreateWorkplace : WorkplaceService
{
    private readonly ApplicationDbContext _ctx;
    public CreateWorkplace(ApplicationDbContext ctx) {
        _ctx = ctx;
    }

    private async Task<bool> WorkplaceExists(string name) {
        if (await _ctx.Workplaces.AnyAsync(x => x.Name == name))
            return true;

        return false;
    }

    public Task<Response> Do(Request request) =>
    TryCatch(async () => {
        var workplaceExists = await WorkplaceExists(request.Name);

        if (workplaceExists) {
            return new Response {
                Message = $"{request.Name} already exists",
                Status = false
            };
        }

        var workplace = new Workplace
            {
            Name = request.Name
        };

        ValidateWorkplaceOnCreate(workplace);

        _ctx.Workplaces.Add(workplace);
        await _ctx.SaveChangesAsync();

        return new Response {
            Message = $"Success {request.Name} is added",
            Status = true
        };
    });

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
