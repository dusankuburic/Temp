using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Temp.Core.Groups.Service;
using Temp.Database;
using Temp.Domain.Models;

namespace Temp.Core.Groups
{
    public class CreateGroup : GroupService
    {
        private readonly ApplicationDbContext _ctx;

        public CreateGroup(ApplicationDbContext ctx) {
            _ctx = ctx;
        }

        private async Task<bool> GroupExists(string name, int organizationId) {
            if (await _ctx.Groups.AnyAsync(x => x.Name == name && x.OrganizationId == organizationId))
                return true;

            return false;
        }

        public Task<Response> Do(Request request) =>
        TryCatch(async () => {
            var groupExists = await GroupExists(request.Name, request.OrganizationId);

            if (groupExists) {
                return new Response {
                    Message = $"Error {request.Name} already exists",
                    Status = false
                };
            }

            var group = new Group
            {
                Name = request.Name,
                OrganizationId = request.OrganizationId
            };

            ValidateGroupOnCreate(group);

            _ctx.Groups.Add(group);
            await _ctx.SaveChangesAsync();

            return new Response {
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

            [Required]
            public int OrganizationId { get; set; }
        }

        public class Response
        {
            public string Message { get; set; }
            public bool Status { get; set; }
        }
    }
}