using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Temp.Database;

namespace Temp.Core.Engagements
{
    public class GetUserEmployeeEngagements : EngagementService
    {
        private readonly ApplicationDbContext _ctx;

        public GetUserEmployeeEngagements(ApplicationDbContext ctx) {
            _ctx = ctx;
        }


        public Task<IEnumerable<Response>> Do(int id) =>
        TryCatch(async () => {

            var user = await _ctx.Users
                .Include(x => x.Employee)
                .Where(x => x.Id == id)
                .FirstOrDefaultAsync();


            ValidateUser(user);

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

            ValidateUserEmployeeEngagements(response);

            return response;
        });


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