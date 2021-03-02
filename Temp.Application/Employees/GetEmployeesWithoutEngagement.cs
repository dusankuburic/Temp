using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Temp.Application.Helpers;
using Temp.Database;

namespace Temp.Application.Employees
{
    public class GetEmployeesWithoutEngagement : EmployeeService
    {
        
        private readonly ApplicationDbContext _ctx;

        public GetEmployeesWithoutEngagement(ApplicationDbContext ctx)
        {
            _ctx = ctx;
        }

        public Task<PagedList<EmployeesWithoutEngagementViewModel>> Do(Request request) =>
        TryCatch(async () =>
        {

            var employeesWithoutEngagement = _ctx.Employees
                .Include(x => x.Engagements)
                .Where(x => x.Engagements.Count == 0)
                .OrderByDescending(x => x.Id)
                .Select(x => new EmployeesWithoutEngagementViewModel
                {
                    Id = x.Id,
                    FirstName = x.FirstName,
                    LastName = x.LastName,
                    Role = x.Role
                }).AsQueryable();
                

            ValidateGetEmployeeWithoutEngagementViewModel(employeesWithoutEngagement);

            return await PagedList<EmployeesWithoutEngagementViewModel>.CreateAsync(employeesWithoutEngagement,
                request.PageNumber, request.PageSize);
        });
        
        public class Request
        {
            private const int MaxPageSize = 20;
            public int PageNumber { get; set; } = 1;

            private int _pageSize = 10;

            public int PageSize
            {
                get { return _pageSize; }
                set { _pageSize = (value > MaxPageSize) ? MaxPageSize : value; }
            }
        }



        public class EmployeesWithoutEngagementViewModel
        {
            public int Id {get; set;}
            public string FirstName {get; set;}
            public string LastName {get; set;}
            public string Role {get; set;}
        }
    }
}
