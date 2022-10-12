using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Temp.Database;

namespace Temp.Core.Organizations;

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
