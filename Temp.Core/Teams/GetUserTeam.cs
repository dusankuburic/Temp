using Temp.Core.Teams.Service;
using Temp.Database;

namespace Temp.Core.Teams;

public class GetUserTeam : TeamService
{
    private readonly ApplicationDbContext _ctx;

    public GetUserTeam(ApplicationDbContext ctx) {
        _ctx = ctx;
    }

    public Task<TeamViewModel> Do(int id) =>
    TryCatch(async () => {
        var team = await _ctx.Users
            .Include(x => x.Employee)
            .Where(x => x.Id == id && x.IsActive)
            .Select(x => new TeamViewModel
            {
                Id = x.Employee.Team.Id,
                Name = x.Employee.Team.Name
            })
            .FirstOrDefaultAsync();

        ValidateGetUserTeamViewModel(team);

        return team;
    });


    public class TeamViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
}
