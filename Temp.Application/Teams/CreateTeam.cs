using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Temp.Database;
using Temp.Domain.Models;

namespace Temp.Application.Teams
{
    public class CreateTeam
    {
        private readonly ApplicationDbContext _ctx;

        public CreateTeam(ApplicationDbContext ctx)
        {
            _ctx = ctx;
        }

        private async Task<bool> TeamExists(string name, int groupId)
        {
            if(await _ctx.Teams.AnyAsync(x => x.Name == name && x.GroupId == groupId))
            {
                return true;
            }
            return false;
        }

        public async Task<Response> Do(Request request)
        {
            var teamExists = await TeamExists(request.Name, request.GroupId);

            if(teamExists)
            {
                return new Response
                {
                    Message = $"Error {request.Name} already exists",
                    Status = false
                };
            }

            var team = new Team
            {
                Name = request.Name,
                GroupId = request.GroupId
            };

            //Validate

            _ctx.Teams.Add(team);
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
            public int Id { get; set; }

            [Required]
            public int GroupId { get; set; }

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
