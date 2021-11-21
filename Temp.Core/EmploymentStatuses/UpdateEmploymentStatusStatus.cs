using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Temp.Database;

namespace Temp.Core.EmploymentStatuses
{
    public class UpdateEmploymentStatusStatus
    {
        private readonly ApplicationDbContext _ctx;

        public UpdateEmploymentStatusStatus(ApplicationDbContext ctx) {
            _ctx = ctx;
        }

        public async Task<bool> Do(int EmploymentStatusId) {
            var employmentStatus = await _ctx.EmploymentStatuses
                .Where(x => x.Id == EmploymentStatusId)
                .FirstOrDefaultAsync();

            employmentStatus.IsActive = false;

            await _ctx.SaveChangesAsync();

            return true;
        }
    }
}