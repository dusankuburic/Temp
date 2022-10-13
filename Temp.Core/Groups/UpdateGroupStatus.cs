using Temp.Database;

namespace Temp.Core.Groups;

public class UpdateGroupStatus
{
    private ApplicationDbContext _ctx;


    public UpdateGroupStatus(ApplicationDbContext ctx) {
        _ctx = ctx;
    }

    public async Task<bool> Do(int GroupId) {
        var group = await _ctx.Groups
                .Where(x => x.Id == GroupId)
                .FirstOrDefaultAsync();

        group.IsActive = false;

        await _ctx.SaveChangesAsync();

        return true;
    }
}

