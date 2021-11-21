using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Temp.Database;

namespace Temp.Core.Workplaces
{
    public class UpdateWorkplaceStatus
    {
        private readonly ApplicationDbContext _ctx;

        public UpdateWorkplaceStatus(ApplicationDbContext ctx) {
            _ctx = ctx;
        }

        public async Task<bool> Do(int WokrplaceId) {
            var workplace = await _ctx.Workplaces
                .Where(x => x.Id == WokrplaceId)
                .FirstOrDefaultAsync();

            workplace.IsActive = false;

            await _ctx.SaveChangesAsync();

            return true;
        }
    }
}