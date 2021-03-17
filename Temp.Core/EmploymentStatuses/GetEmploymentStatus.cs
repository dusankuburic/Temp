using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using Temp.Core.EmploymentStatuses.Service;
using Temp.Database;

namespace Temp.Core.EmploymentStatuses
{
    public class GetEmploymentStatus : EmploymentStatusService
    {
        private readonly ApplicationDbContext _ctx;

        public GetEmploymentStatus(ApplicationDbContext ctx)
        {
            _ctx = ctx;
        }

        public Task<EmploymentStatusViewModel> Do(int id) =>
        TryCatch(async() => 
        { 
             var employmentStatus = await _ctx.EmploymentStatuses
            .Where(x => x.Id == id)
            .Select(x => new EmploymentStatusViewModel
            {
                Id = x.Id,
                Name = x.Name
            })
            .FirstOrDefaultAsync();

            ValidateGetEmploymentStatus(employmentStatus);

            return employmentStatus;
        });

        public class EmploymentStatusViewModel
        {
            public int Id {get; set;}
            public string Name {get; set;}
        }
    }
}
 