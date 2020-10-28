using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Temp.Database;
using Temp.Domain.Models;

namespace Temp.Application.EmploymentStatuses
{
    public class CreateEmploymentStatus
    {
        private readonly ApplicationDbContext _ctx;


        public CreateEmploymentStatus(ApplicationDbContext ctx)
        {
            _ctx = ctx;
        }

        public async Task<Response> Do(Request request)
        {
            var employmentStatus = new EmploymentStatus
            {
                Name = request.Name
            };

            _ctx.EmploymentStatuses.Add(employmentStatus);
            await _ctx.SaveChangesAsync();

            return new Response
            {
                Message = $"Success {request.Name} is added",
                Status = true
            };
        }

        public class Request
        {
         
            [Required]
            [MaxLength(30)]
            public string Name {get; set;}
        }

        public class Response
        {
            public string Message {get; set;}
            public bool Status {get; set;}
        }


    }
}
