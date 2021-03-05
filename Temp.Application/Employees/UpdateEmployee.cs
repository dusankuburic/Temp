using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Temp.Database;

namespace Temp.Application.Employees
{
    public class UpdateEmployee : EmployeeService
    {

        private readonly ApplicationDbContext _ctx;
        public UpdateEmployee(ApplicationDbContext ctx)
        {
            _ctx = ctx;
        }

        public Task<Response> Do(int id ,Request request) =>
        TryCatch(async () => 
        {
            var employee = _ctx.Employees.FirstOrDefault(x => x.Id == id);

            employee.FirstName = request.FirstName;
            employee.LastName = request.LastName;
            employee.TeamId = request.TeamId;

            ValidateEmployeeOnUpdate(employee);

            await _ctx.SaveChangesAsync();

            return new Response()
            {
                Id = employee.Id,
                FirstName = employee.FirstName,
                LastName = employee.LastName,
                Status = true
            };
        });

     
        public class Request
        {
            [Required]
            [MaxLength(30)]
            public string FirstName {get; set;}
            
            [Required]
            [MaxLength(30)]
            public string LastName {get; set;}

            [Required] 
            public int TeamId { get; set; }
        }

        public class Response
        {
            public int Id {get; set;}
            public string FirstName {get; set;}
            public string LastName {get; set;}
            public bool Status {get; set;}
        }
    }
}
