using Temp.Database;

namespace Temp.Services.Organizations.CLI.Command;

public class UpdateOrganizationStatus
{
    private readonly ApplicationDbContext _ctx;

    public UpdateOrganizationStatus(ApplicationDbContext ctx) {
        _ctx = ctx;
    }

    public async Task<bool> Do(int OrganizationId) {
        var ortanization = await _ctx.Organizations
            .Where(x => x.Id == OrganizationId)
            .FirstOrDefaultAsync();

        ortanization.IsActive = false;

        await _ctx.SaveChangesAsync();

        return true;
    }
}

