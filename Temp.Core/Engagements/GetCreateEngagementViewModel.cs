using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Temp.Core.Employees;
using Temp.Core.EmploymentStatuses;
using Temp.Core.Workplaces;
using Temp.Database;

namespace Temp.Core.Engagements
{
    public class GetCreateEngagementViewModel : EngagementService
    {
        private readonly ApplicationDbContext _ctx;

        public GetCreateEngagementViewModel(ApplicationDbContext ctx) {
            _ctx = ctx;
        }

        public Task<string> Do(int id) =>
        TryCatch(async () => {
            var response = new Response
            {
                Employee = await new GetEmployee(_ctx).Do(id),
                Workplaces = await new GetWorkplaces(_ctx).Do(),
                EmploymentStatuses = await new GetEmploymentStatuses(_ctx).Do(),
                Engagements = await _ctx.Engagements.Where(x => x.Employee.Id == id)
                    .Select(eng => new
                    {
                        id = eng.Id,
                        workplaceName = eng.Workplace.Name,
                        employmentStatusName = eng.EmploymentStatus.Name,
                        dateFrom = eng.DateFrom,
                        dateTo = eng.DateTo,
                        salary = eng.Salary
                    }).ToListAsync()
            };

            ValidateCreateEngagementViewModel(response);

            return JsonConvert.SerializeObject(response);
        });

        public class Response
        {
            public GetEmployee.EmployeeViewModel Employee;
            public IEnumerable<GetWorkplaces.WorkplacesViewModel> Workplaces;
            public IEnumerable<GetEmploymentStatuses.EmploymentStatusViewModel> EmploymentStatuses;
            public IEnumerable Engagements;
        }
    }
}