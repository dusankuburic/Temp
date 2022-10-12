using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Temp.Database;
using Temp.Domain.Models;

namespace Temp.Core.Employees;

public class CreateEmployee : EmployeeService
{
    private readonly ApplicationDbContext _ctx;


    public CreateEmployee(ApplicationDbContext ctx) {
        _ctx = ctx;
    }

    public Task<Response> Do(Request request) =>
    TryCatch(async () => {
        var employee = new Employee
            {
            FirstName = request.FirstName,
            LastName =  request.LastName,
            TeamId = request.TeamId
        };

        ValidateEmployeeOnCreate(employee);

        _ctx.Employees.Add(employee);
        await _ctx.SaveChangesAsync();

        return new Response {
            Message = $"Success {employee.FirstName} {employee.LastName} is added"
        };
    });

    public class Request
    {
        [Required]
        [MaxLength(30)]
        public string FirstName { get; set; }

        [Required]
        [MaxLength(30)]
        public string LastName { get; set; }

        [Required]
        public int TeamId { get; set; }
    }

    public class Response
    {
        public string Message { get; set; }
    }
}
