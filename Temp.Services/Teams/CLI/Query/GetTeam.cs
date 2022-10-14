using Temp.Database;

namespace Temp.Services.Teams.CLI.Query;

public class GetTeam
{
    private  readonly ApplicationDbContext _ctx;

    public GetTeam(ApplicationDbContext ctx) {
        _ctx = ctx;
    }

    public async Task<TeamViewModel> Do(int id) {
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

        return team;
    }

    public class TeamViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int GroupId { get; set; }
    }
}

