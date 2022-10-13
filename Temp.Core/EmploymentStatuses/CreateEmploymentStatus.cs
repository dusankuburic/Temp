using Temp.Core.EmploymentStatuses.Service;
using Temp.Database;
using Temp.Domain.Models;

namespace Temp.Core.EmploymentStatuses;

public class CreateEmploymentStatus : EmploymentStatusService
{
    private readonly ApplicationDbContext _ctx;

    public CreateEmploymentStatus(ApplicationDbContext ctx) {
        _ctx = ctx;
    }

    private async Task<bool> EmploymentStatusExists(string name) {
        if (await _ctx.EmploymentStatuses.AnyAsync(x => x.Name == name))
            return true;

        return false;
    }

    public Task<Response> Do(Request request) =>
    TryCatch(async () => {

        var employmentStatusExists = await EmploymentStatusExists(request.Name);

        if (employmentStatusExists) {
            return new Response {
                Message = $"Error {request.Name} already exists",
                Status = false
            };
        }

        var employmentStatus = new EmploymentStatus
            {
            Name = request.Name
        };

        ValidateEmploymentStatusOnCreate(employmentStatus);

        _ctx.EmploymentStatuses.Add(employmentStatus);
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
        [MaxLength(30)]
        public string Name { get; set; }
    }

    public class Response
    {
        public string Message { get; set; }
        public bool Status { get; set; }
    }
}
