using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Internal;
using Temp.Application.Auth.Users;
using Temp.Application.Helpers;
using Temp.Database;

namespace Temp.Application.Employees
{
    public class GetEmployeesWithEngagement : EmployeeService
    {
        
        private readonly ApplicationDbContext _ctx;

        public GetEmployeesWithEngagement(ApplicationDbContext ctx)
        {
            _ctx = ctx;
        }

        public Task<PagedList<EmployeesWithEngagementViewModel>> Do(Request request) =>
        TryCatch(async () =>
        {

            var employeesWithEngagement = _ctx.Employees
                .Include(x => x.Engagements)
                .Where(x => x.Engagements.Count != 0)
                .OrderByDescending(x => x.Id)
                .Select(x => new EmployeesWithEngagementViewModel
                {
                    Id = x.Id,
                    FirstName = x.FirstName,
                    LastName = x.LastName,
                    Role = x.Role
                }).AsQueryable();

            if (!string.IsNullOrEmpty(request.Workplace) && !string.IsNullOrEmpty(request.EmploymentStatus))
            {
                employeesWithEngagement = employeesWithEngagement
                    .Join(_ctx.Engagements,
                        employee => employee.Id,
                        engagement => engagement.EmployeeId,
                        (employee, engagement) => new EmployeesWithEngagementViewModel
                        {
                            Id = employee.Id,
                            FirstName = employee.FirstName,
                            LastName = employee.LastName,
                            Role = employee.Role,
                            Workplace = engagement.Workplace.Name,
                            EmploymentStatus = engagement.EmploymentStatus.Name,
                        })
                    .Where(x => x.Workplace.Contains(request.Workplace) && x.EmploymentStatus.Contains(request.EmploymentStatus))
                    .AsQueryable();
            }
            else if (!string.IsNullOrEmpty(request.Workplace))
            {
                employeesWithEngagement = employeesWithEngagement
                    .Join(_ctx.Engagements,
                        employee => employee.Id,
                        engagement => engagement.EmployeeId,
                        (employee, engagement) => new EmployeesWithEngagementViewModel
                        {
                            Id = employee.Id,
                            FirstName = employee.FirstName,
                            LastName = employee.LastName,
                            Role = employee.Role,
                            Workplace = engagement.Workplace.Name,
                            EmploymentStatus = engagement.EmploymentStatus.Name,
                        })
                    .Where(x => x.Workplace.Contains(request.Workplace))
                    .AsQueryable();
            }
            else if (!string.IsNullOrEmpty(request.EmploymentStatus))
            {
                employeesWithEngagement = employeesWithEngagement
                    .Join(_ctx.Engagements,
                        employee => employee.Id,
                        engagement => engagement.EmployeeId,
                        (employee, engagement) => new EmployeesWithEngagementViewModel
                        {
                            Id = employee.Id,
                            FirstName = employee.FirstName,
                            LastName = employee.LastName,
                            Role = employee.Role,
                            Workplace = engagement.Workplace.Name,
                            EmploymentStatus = engagement.EmploymentStatus.Name
                        })
                    .Where(x => x.EmploymentStatus.Contains(request.EmploymentStatus))
                    .AsQueryable();
            }

            ValidateGetEmployeeWithEngagementViewModel(employeesWithEngagement);

            return await PagedList<EmployeesWithEngagementViewModel>.CreateAsync(employeesWithEngagement, 
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
            
            public string Workplace { get; set; }
            public string EmploymentStatus { get; set; }
        }
        
        public class EmployeesWithEngagementViewModel
        {
            public int Id {get; set;}
            public string FirstName {get; set;}
            public string LastName {get; set;}
            public string Role {get; set;}

            public string Workplace { get; set; } = string.Empty;
            public string EmploymentStatus { get; set; } = string.Empty;
        }
    }
}
