using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Temp.Database;

namespace Temp.Application.Empolyees
{
    public class UpdateEmployee
    {

        private readonly ApplicationDbContext _ctx;
        public UpdateEmployee(ApplicationDbContext ctx)
        {
            _ctx = ctx;
        }

        public async Task<Response> Do(Request request)
        {
            var employee = _ctx.Employees.FirstOrDefault(x => x.Id == request.Id);

            employee.FirstName = request.FirstName;
            employee.LastName = request.LastName;

            await _ctx.SaveChangesAsync();

            return new Response()
            {
                Id = employee.Id,
                FirstName = employee.FirstName,
                LastName = employee.LastName,
                Message = "Success",
                Status = true
            };
        }
     
        public class Request
        {
            [Required]
            public int Id {get; set;}
            [Required]
            [MaxLength(30)]
            public string FirstName {get; set;}
            [Required]
            [MaxLength(30)]
            public string LastName {get; set;}
        }

        public class Response
        {
            public int Id {get; set;}
            public string FirstName {get; set;}
            public string LastName {get; set;}
            public string Message {get; set;}
            public bool Status {get; set;}
        }
    }
}
