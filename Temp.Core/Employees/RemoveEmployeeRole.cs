using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Temp.Database;

namespace Temp.Core.Employees;

public class RemoveEmployeeRole
{
    private readonly ApplicationDbContext _ctx;

    public RemoveEmployeeRole(ApplicationDbContext ctx) {
        _ctx = ctx;
    }

    public async Task<Response> Do(Request request) {
        var employee = _ctx.Employees.FirstOrDefault(x => x.Id == request.Id);
        string message = "";

        if (employee.Role == "Admin") {
            var admin = _ctx.Admins.Where(x => x.EmployeeId == request.Id).FirstOrDefault();
            _ctx.Admins.Remove(admin);
            message = $"Removed Admin role for Id:{employee.Id} {employee.FirstName} {employee.LastName}";
        }

        if (employee.Role == "User") {
            var user = _ctx.Users.Where(x => x.EmployeeId == request.Id).FirstOrDefault();
            _ctx.Users.Remove(user);
            message = $"Removed User role for Id:{employee.Id} {employee.FirstName} {employee.LastName}";
        }

        if (employee.Role == "Moderator") {
            var moderator = _ctx.Moderators.Where(x => x.EmployeeId == request.Id).FirstOrDefault();
            _ctx.Moderators.Remove(moderator);
        }


        employee.Role = "None";
        await _ctx.SaveChangesAsync();

        return new Response {
            Message = message,
            Status = true
        };
    }

    public class Request
    {
        [Required]
        public int Id { get; set; }
    }

    public class Response
    {
        public string Message { get; set; }
        public bool Status { get; set; }
    }
}
