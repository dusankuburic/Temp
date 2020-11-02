using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Temp.Database;
using Temp.Domain.Models;

namespace Temp.Application.Empolyees
{
    public class CreateEmployee: EmployeeService
    {
        private readonly ApplicationDbContext _ctx;
 
       
        public CreateEmployee(ApplicationDbContext ctx)
        {
            _ctx = ctx;
  
        }

        public Task<Response> Do(Request request) =>
        TryCatch(async () =>
        {

            var employee = new Employee
            {
                FirstName = request.FirstName,
                LastName =  request.LastName
            };


            ValidateEmployeeOnCreate(employee);

            _ctx.Employees.Add(employee);
            await _ctx.SaveChangesAsync();

            return new Response
            {
                Message = $"Success {employee.FirstName} {employee.LastName} is added",
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
        }

        public class Response
        {
            public string Message {get; set;}
            public bool Status {get; set;}
        }
    }
}
