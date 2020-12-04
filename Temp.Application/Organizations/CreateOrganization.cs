using Microsoft.EntityFrameworkCore;
using Temp.Application.Organizations.Service;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Temp.Database;
using Temp.Domain.Models;

namespace Temp.Application.Organizations
{
    public class CreateOrganization :  OrganizationService
    {
        private readonly ApplicationDbContext _ctx;

        public CreateOrganization(ApplicationDbContext ctx)
        {
            _ctx = ctx;
        }

        private async Task<bool> OrganizationExists(string name)
        {
            if(await _ctx.Organizations.AnyAsync(x => x.Name == name))
            {
                return true;
            }
            return false;
        }

        public Task<Response> Do(Request request) =>
        TryCatch(async () =>
        {

            var organizationExists = await OrganizationExists(request.Name);

            if (organizationExists)
            {
                return new Response
                {
                    Message = $"Error {request.Name} already exists",
                    Status = false
                };
            }

            var organization = new Organization
            {
                Name = request.Name
            };

            ValidateOrganizationOnCreate(organization);

            _ctx.Organizations.Add(organization);
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
            [MaxLength(50)]
            public string Name { get; set; }
        }

        public class Response
        {
            public string Message { get; set; }
            public bool Status { get; set; }
        }
    }
}
