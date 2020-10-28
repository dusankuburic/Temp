using System.Collections.Generic;
using System.Linq;
using Temp.Database;

namespace Temp.Application.EmploymentStatuses
{
    public class GetEmploymentStatuses
    {
        private readonly ApplicationDbContext _ctx;

        public GetEmploymentStatuses(ApplicationDbContext ctx)
        {
            _ctx = ctx;
        }

        public IEnumerable<EmploymentStatusViewModel> Do() =>
            _ctx.EmploymentStatuses.ToList().Select(x => new EmploymentStatusViewModel
            {
                Id = x.Id,
                Name = x.Name
            });

        public class EmploymentStatusViewModel
        {
            public int Id {get; set;}
            public string Name {get; set;}
        }
    }
}
