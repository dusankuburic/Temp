using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Temp.Core.Helpers;
using Temp.Database;

namespace Temp.Core.Employees;

public class GetEmployeesWithEngagement : EmployeeService
{
    private readonly ApplicationDbContext _ctx;

    public GetEmployeesWithEngagement(ApplicationDbContext ctx) {
        _ctx = ctx;
    }

    public Task<PagedList<EmployeesWithEngagementViewModel>> Do(Request request) =>
        TryCatch(async () => {
            var currentDateTime = DateTime.Now;

            var employeesWithEngagement = _ctx.Employees
                    .Include(x => x.Engagements)
                    .Where(x => x.Engagements.Count != 0)
                    .Where(x => x.Engagements.Any(n => n.DateTo > currentDateTime))
                    .OrderByDescending(x => x.Id)
                    .Select(x => new EmployeesWithEngagementViewModel
                    {
                        Id = x.Id,
                        FirstName = x.FirstName,
                        LastName = x.LastName,
                        Role = x.Role,
                        Salary = _ctx.Engagements
                            .Where(e => e.EmployeeId == x.Id)
                            .Select(e => e.Salary)
                            .ToList(),
                        Workplace = _ctx.Engagements
                            .Where(e => e.EmployeeId == x.Id)
                            .Select(e => e.Workplace.Name)
                            .ToList(),
                        EmploymentStatus = _ctx.Engagements
                            .Where(e => e.EmployeeId == x.Id)
                            .Select(e => e.EmploymentStatus.Name)
                            .ToList()
                    })
                   .AsQueryable();

            if (request.MinSalary != 0 || request.MaxSalary != 5000) {
                employeesWithEngagement = employeesWithEngagement
                    .Where(x => x.Salary.All(sal => sal >= request.MinSalary && sal <= request.MaxSalary))
                    .AsQueryable();
            }

            if (!string.IsNullOrEmpty(request.Workplace) && !string.IsNullOrEmpty(request.EmploymentStatus)) {
                employeesWithEngagement = employeesWithEngagement
                    .Where(x => x.Workplace.Any(w => w.Contains(request.Workplace)) &&
                                x.EmploymentStatus.Any(e => e.Contains(request.EmploymentStatus)))
                    .AsQueryable();
            } else if (!string.IsNullOrEmpty(request.Workplace)) {
                employeesWithEngagement = employeesWithEngagement
                    .Where(x => x.Workplace.Any(w => w.Contains(request.Workplace)))
                    .AsQueryable();

            } else if (!string.IsNullOrEmpty(request.EmploymentStatus)) {
                employeesWithEngagement = employeesWithEngagement
                    .Where(x => x.EmploymentStatus.Any(es => es.Contains(request.EmploymentStatus)))
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

        public int MinSalary { get; set; } = 0;
        public int MaxSalary { get; set; } = 5000;
    }

    public class EmployeesWithEngagementViewModel
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Role { get; set; }

        public IEnumerable<string> Workplace { get; set; }
        public IEnumerable<string> EmploymentStatus { get; set; }
        public IEnumerable<int> Salary { get; set; }
    }
}
