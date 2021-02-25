using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Temp.Application.Organizations.Service;
using Temp.Database;

namespace Temp.Application.Organizations
{
    public class UpdateOrganization : OrganizationService
    {
        private readonly ApplicationDbContext _ctx;

        public UpdateOrganization(ApplicationDbContext ctx)
        {
            _ctx = ctx;
        }

        public async Task<bool> OrganizationExists(string name)
        {
            if(await _ctx.Organizations.AnyAsync(x => x.Name == name))
            {
                return true;
            }
            return false;
        }

        public Task<Response> Do(int id, Request request) =>
        TryCatch(async () =>
        {
            var organization = _ctx.Organizations.FirstOrDefault(x => x.Id == id);

            if(organization.Name.Equals(request.Name))
            {
                return new Response
                {
                    Id = organization.Id,
                    Name = organization.Name,
                    Message = "Organization name is same",
                    Status = true
                };
            }

            var organizationExists = await OrganizationExists(request.Name);

            if(organizationExists)
            {
                return new Response
                {
                    Id = organization.Id,
                    Name = organization.Name,
                    Message = $"Organization already exists with {request.Name} name",
                    Status = false
                };
            }

            organization.Name = request.Name;

            ValidateOrganizationOnUpdate(organization);

            await _ctx.SaveChangesAsync();

            return new Response
            {
                Id = organization.Id,
                Name = organization.Name,
                Message = "Success",
                Status = true
            };

        });
      
        public class Request
        {                
            [MinLength(2)]
            [MaxLength(50)]
            public string Name { get; set; }
        }

        public class Response
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public string Message { get; set; }
            public bool Status { get; set; }
        }

    }
}
