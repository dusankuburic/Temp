using Temp.Database;

namespace Temp.Core.Teams;

public class UpdateTeamStatus
{
    private ApplicationDbContext _ctx;

    public UpdateTeamStatus(ApplicationDbContext ctx) {
        _ctx = ctx;
    }

    public async Task<bool> Do(int TeamId) {
        var team = await _ctx.Teams
                .Where(x => x.Id == TeamId)
                .FirstOrDefaultAsync();

        team.IsActive = false;

        await _ctx.SaveChangesAsync();

        return true;
    }
}
