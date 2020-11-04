using System.Linq;
using Temp.Database;

namespace Temp.Application.EmploymentStatuses
{
    public class GetEmploymentStatus
    {
        private readonly ApplicationDbContext _ctx;

        public GetEmploymentStatus(ApplicationDbContext ctx)
        {
            _ctx = ctx;
        }

        public EmploymentStatusViewModel Do(int id) =>
            _ctx.EmploymentStatuses.Where(x => x.Id == id)
            .Select(x => new EmploymentStatusViewModel
            {
                Id = x.Id,
                Name = x.Name
            })
            .FirstOrDefault();
        
        public class EmploymentStatusViewModel
        {
            public int Id {get; set;}
            public string Name {get; set;}
        }
    }
}
 