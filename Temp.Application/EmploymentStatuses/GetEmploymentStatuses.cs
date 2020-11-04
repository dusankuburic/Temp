using System.Collections.Generic;
using System.Linq;
using Temp.Application.EmploymentStatuses.Service;
using Temp.Database;

namespace Temp.Application.EmploymentStatuses
{
    public class GetEmploymentStatuses : EmploymentStatusService
    {
        private readonly ApplicationDbContext _ctx;

        public GetEmploymentStatuses(ApplicationDbContext ctx)
        {
            _ctx = ctx;
        }

        public IEnumerable<EmploymentStatusViewModel> Do() =>
        TryCatch(() =>
        {
            var employmentStatuses = _ctx.EmploymentStatuses.ToList()
            .Select(x => new EmploymentStatusViewModel
            {
                Id = x.Id,
                Name = x.Name
            });

            ValidateEmploymentStatuses(employmentStatuses);

            return employmentStatuses;
        });

        public class EmploymentStatusViewModel
        {
            public int Id {get; set;}
            public string Name {get; set;}
        }
    }
}
