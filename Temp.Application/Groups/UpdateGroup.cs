using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Temp.Application.Groups.Service;
using Temp.Database;

namespace Temp.Application.Groups
{
    public class UpdateGroup : GroupService
    {
        private readonly ApplicationDbContext _ctx;
        
        public UpdateGroup(ApplicationDbContext ctx)
        {
            _ctx = ctx;
        }

        private async Task<bool> GroupExists(string name, int organizationId)
        {
            if (await _ctx.Groups.AnyAsync(x => x.Name == name && x.OrganizationId == organizationId))           
                return true;
            
            return false;
        }

        public Task<Response> Do(int id, Request request) =>
        TryCatch(async () =>
        {
            var group = _ctx.Groups.FirstOrDefault(x => x.Id == id);

            if (group.Name.Equals(request.Name))
            {
                return new Response
                {
                    Id = group.Id,
                    Name = group.Name,
                    Message = "Group name is same",
                    Status = true
                };
            }

            var groupExists = await GroupExists(request.Name, request.OrganizationId);

            if (groupExists)
            {
                return new Response
                {
                    Message = $"Error {request.Name} already exists",
                    Status = false
                };
            }

            group.Name = request.Name;

            ValidateGroupOnUpdate(group);

            await _ctx.SaveChangesAsync();

            return new Response
            {
                Id = group.Id,
                Name = group.Name,
                Message = "Success",
                Status = true
            };
        });

        public class Request
        {
            [Required]
            public int OrganizationId { get; set; }

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
