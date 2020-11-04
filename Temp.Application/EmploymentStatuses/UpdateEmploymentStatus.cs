using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Temp.Application.EmploymentStatuses.Service;
using Temp.Database;

namespace Temp.Application.EmploymentStatuses
{
    public class UpdateEmploymentStatus : EmploymentStatusService
    {
        private readonly ApplicationDbContext _ctx;

        public UpdateEmploymentStatus(ApplicationDbContext ctx)
        {
            _ctx = ctx;
        }

        private async Task<bool> EmploymentStatusExists(string name)
        {
            if(await _ctx.EmploymentStatuses.AnyAsync(x => x.Name == name))
            {
                return true;
            }
            return false;
        }

        public Task<Response> Do(Request request) =>
        TryCatch(async() =>
        {
            var employmentStatus = _ctx.EmploymentStatuses.FirstOrDefault(x => x.Id == request.Id);

            if (employmentStatus.Name.Equals(request.Name))
            {
                return new Response
                {
                    Id = employmentStatus.Id,
                    Name = employmentStatus.Name,
                    Message = "Employment Status is same",
                    Status = false
                };
            }

            var employmentStatusExists = await EmploymentStatusExists(request.Name);

            if(employmentStatusExists)
            {
                return new Response
                {
                    Id = employmentStatus.Id,
                    Name = employmentStatus.Name,
                    Message = $"Employment Status already exists with {request.Name} name",
                    Status = false
                };
            }

            employmentStatus.Name = request.Name;

            ValidateEmploymentStatusOnUpdate(employmentStatus);

            await _ctx.SaveChangesAsync();

            return new Response
            {
                Id = employmentStatus.Id,
                Name = employmentStatus.Name,
                Message = "Success",
                Status = true
            };
        });

        public class Request
        {
            [Required]
            public int Id {get; set;}

            [Required]
            [MaxLength(30)]
            public string Name {get; set;}
        }

        public class Response
        {
            public int Id {get; set;}
            public string Name {get; set;}
            public string Message {get; set;}
            public bool Status {get; set;}
        }
    }
}
