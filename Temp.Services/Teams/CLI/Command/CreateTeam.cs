using Temp.Database;
using Temp.Domain.Models;

namespace Temp.Services.Teams.CLI.Command
{
    public class CreateTeam
    {
        private readonly ApplicationDbContext _ctx;

        public CreateTeam(ApplicationDbContext ctx) {
            _ctx = ctx;
        }

        public async Task<Response> Do(Team team) {
            _ctx.Teams.Add(team);
            await _ctx.SaveChangesAsync();

            return new Response {
                Message = $"Success {team.Name} is added",
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
