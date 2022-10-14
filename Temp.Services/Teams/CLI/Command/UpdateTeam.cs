using Temp.Database;
using Temp.Domain.Models;

namespace Temp.Services.Teams.CLI.Command;

public class UpdateTeam
{
    private readonly ApplicationDbContext _ctx;

    public UpdateTeam(ApplicationDbContext ctx) {
        _ctx = ctx;
    }


    public async Task<Response> Do(Team team) {

        await _ctx.SaveChangesAsync();

        return new Response {
            Id = team.Id,
            Name = team.Name,
            Message = "Success",
            Status = true
        };
    }

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
