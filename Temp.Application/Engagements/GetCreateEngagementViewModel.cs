using System.Collections;
using System.Collections.Generic;
using Temp.Application.EmploymentStatuses;
using Temp.Application.Employees;
using Temp.Application.Workplaces;
using Temp.Database;
using System.Linq;
using Newtonsoft.Json;

namespace Temp.Application.Engagements
{
    public class GetCreateEngagementViewModel : EngagementService
    {
        private readonly ApplicationDbContext _ctx;

        public GetCreateEngagementViewModel(ApplicationDbContext ctx)
        {
            _ctx = ctx;
        }
       
        public string Do(int id) =>
        TryCatch(() => 
        { 
            var response = new Response
            {
                Employee = new GetEmployee(_ctx).Do(id),
                Workplaces = new GetWorkplaces(_ctx).Do(),
                EmploymentStatuses = new GetEmploymentStatuses(_ctx).Do(),
                Engagements = _ctx.Engagements.Where(x => x.Employee.Id == id)
                    .Select(eng => new
                    {
                       id = eng.Id,
                       workplaceName = eng.Workplace.Name,
                       employmentStatusName = eng.EmploymentStatus.Name,
                       dateFrom = eng.DateFrom,
                       dateTo = eng.DateTo
                    })
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
