using System.Linq;
using Temp.Application.EmploymentStatuses.Service;
using Temp.Database;

namespace Temp.Application.EmploymentStatuses
{
    public class GetEmploymentStatus : EmploymentStatusService
    {
        private readonly ApplicationDbContext _ctx;

        public GetEmploymentStatus(ApplicationDbContext ctx)
        {
            _ctx = ctx;
        }

        public EmploymentStatusViewModel Do(int id) =>
        TryCatch(() => 
        { 
             var employmentStatus = _ctx.EmploymentStatuses
            .Where(x => x.Id == id)
            .Select(x => new EmploymentStatusViewModel
            {
                Id = x.Id,
                Name = x.Name
            })
            .FirstOrDefault();

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
 