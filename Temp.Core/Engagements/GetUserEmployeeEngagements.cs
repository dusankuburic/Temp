using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Temp.Database;

namespace Temp.Core.Engagements
{
    public class GetUserEmployeeEngagements
    {
        private readonly ApplicationDbContext _ctx;

        public GetUserEmployeeEngagements(ApplicationDbContext ctx)
        {
            _ctx = ctx;
        }


        public async Task<IEnumerable<Response>> Do(int id)
        {
            var user = await _ctx.Users
                .Include(x => x.Employee)
                .Where(x => x.Id == id)
                .FirstOrDefaultAsync();

            //Validate employee

            var response = await _ctx.Engagements
                .Include(x => x.Workplace)
                .Include(x => x.EmploymentStatus)
                .Where(x => x.EmployeeId == user.Employee.Id)
                .Select(x => new Response
                {
                    WorkplaceName = x.Workplace.Name,
                    EmploymentStatusName = x.EmploymentStatus.Name,
                    DateFrom = x.DateFrom,
                    DateTo = x.DateTo,
                    Salary = x.Salary
                })
                .ToListAsync();

            //Validate response(engagements)

            return response;
        }

        public class Response
        {
            public string WorkplaceName { get; set; }
            public string EmploymentStatusName { get; set; }
            public DateTime DateFrom { get; set; }
            public DateTime DateTo { get; set; }
            public int Salary { get; set; }
        }
    }
}
