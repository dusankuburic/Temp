using Temp.Database;

namespace Temp.Services.Teams.CLI.Query;

public class GetUserTeam
{
    private readonly ApplicationDbContext _ctx;

    public GetUserTeam(ApplicationDbContext ctx) {
        _ctx = ctx;
    }

    public async Task<TeamViewModel> Do(int id) {
        var team = await _ctx.Users
            .Include(x => x.Employee)
            .Where(x => x.Id == id && x.IsActive)
            .Select(x => new TeamViewModel
            {
                Id = x.Employee.Team.Id,
                Name = x.Employee.Team.Name
            })
            .FirstOrDefaultAsync();

        return team;
    }

    public class TeamViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
}
