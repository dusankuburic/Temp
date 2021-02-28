using Microsoft.EntityFrameworkCore;
using System.Linq;
using Temp.Application.Teams.Service;
using Temp.Database;

namespace Temp.Application.Teams
{
    public class GetTeam : TeamService
    {
        private  readonly ApplicationDbContext _ctx;

        public GetTeam(ApplicationDbContext ctx)
        {
            _ctx = ctx;
        }

        public TeamViewModel Do(int id) =>
        TryCatch(() =>
        {
            var team = _ctx.Teams.Include(x => x.Group)
                .Where(x => x.Id == id)
                .Select(x => new TeamViewModel
                {
                    Id = x.Id,
                    Name = x.Name,
                    GroupId = x.Group.Id
                })
                .FirstOrDefault();

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
