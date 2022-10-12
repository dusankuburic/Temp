using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Temp.Core.Teams.Service;
using Temp.Database;

namespace Temp.Core.Teams;

public class UpdateTeam : TeamService
{
    private readonly ApplicationDbContext _ctx;

    public UpdateTeam(ApplicationDbContext ctx) {
        _ctx = ctx;
    }

    private async Task<bool> TeamExists(string name, int groupId) {
        if (await _ctx.Teams.AnyAsync(x => x.Name == name && x.GroupId == groupId))
            return true;

        return false;
    }

    public Task<Response> Do(int id, Request request) =>
    TryCatch(async () => {
        var team = _ctx.Teams.FirstOrDefault(x => x.Id == id);


        if (team.Name.Equals(request.Name)) {
            return new Response {
                Id = team.Id,
                Name = team.Name,
                Message = "Team name is same",
                Status = true
            };
        }

        var teamExists = await TeamExists(request.Name, request.GroupId);

        if (teamExists) {
            return new Response {
                Message = $"{request.Name} already exists",
                Status = false
            };
        }

        team.Name = request.Name;

        ValidateTeamOnUpdate(team);

        await _ctx.SaveChangesAsync();

        return new Response {
            Id = team.Id,
            Name = team.Name,
            Message = "Success",
            Status = true
        };
    });

    public class Request
    {
        [Required]
        public int GroupId { get; set; }

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
