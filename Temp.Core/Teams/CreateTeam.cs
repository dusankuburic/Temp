using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Temp.Core.Teams.Service;
using Temp.Database;
using Temp.Domain.Models;

namespace Temp.Core.Teams;

public class CreateTeam : TeamService
{
    private readonly ApplicationDbContext _ctx;

    public CreateTeam(ApplicationDbContext ctx) {
        _ctx = ctx;
    }

    private async Task<bool> TeamExists(string name, int groupId) {
        if (await _ctx.Teams.AnyAsync(x => x.Name == name && x.GroupId == groupId))
            return true;

        return false;
    }

    public Task<Response> Do(Request request) =>
    TryCatch(async () => {
        var teamExists = await TeamExists(request.Name, request.GroupId);

        if (teamExists) {
            return new Response {
                Message = $"Error {request.Name} already exists",
                Status = false
            };
        }

        var team = new Team
            {
            Name = request.Name,
            GroupId = request.GroupId
        };

        ValidateTeamOnCreate(team);

        _ctx.Teams.Add(team);
        await _ctx.SaveChangesAsync();

        return new Response {
            Message = $"Success {request.Name} is added",
            Status = true
        };
    });

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
