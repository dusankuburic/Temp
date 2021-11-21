using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Temp.Core.Teams.Service;
using Temp.Database;

namespace Temp.Core.Teams
{
    public class GetTeam : TeamService
    {
        private  readonly ApplicationDbContext _ctx;

        public GetTeam(ApplicationDbContext ctx) {
            _ctx = ctx;
        }

        public Task<TeamViewModel> Do(int id) =>
        TryCatch(async () => {
            var team = await _ctx.Teams
                .Include(x => x.Group)
                .Where(x => x.Id == id && x.IsActive)
                .Select(x => new TeamViewModel
                {
                    Id = x.Id,
                    Name = x.Name,
                    GroupId = x.Group.Id
                })
                .FirstOrDefaultAsync();

            ValidateGetTeamViewModel(team);

            return team;
        });

        public class TeamViewModel
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public int GroupId { get; set; }
        }
    }
}