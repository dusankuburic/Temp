using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Temp.Application.EmploymentStatuses.Service;
using Temp.Database;
using Temp.Domain.Models;

namespace Temp.Application.EmploymentStatuses
{
    public class CreateEmploymentStatus : EmploymentStatusService
    {
        private readonly ApplicationDbContext _ctx;

        public CreateEmploymentStatus(ApplicationDbContext ctx)
        {
            _ctx = ctx;
        }

        public Task<Response> Do(Request request) =>
        TryCatch(async () => 
        {
            var employmentStatus = new EmploymentStatus
            {
                Name = request.Name
            };

            ValidateEmploymentStatusOnCreate(employmentStatus);

            _ctx.EmploymentStatuses.Add(employmentStatus);
            await _ctx.SaveChangesAsync();

            return new Response
            {
                Message = $"Success {request.Name} is added",
                Status = true
            };
        });

        public class Request
        {
         
            [Required]
            [MinLength(2)]
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
