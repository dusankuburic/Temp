using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
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

        public IEnumerable<EmployeesWithEngagementViewModel> Do() =>
        TryCatch(() =>
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
            })
            .ToList();

            ValidateGetEmployeeWithEngagementViewModel(employeesWithEngagement);

            return employeesWithEngagement;
        });



        public class EmployeesWithEngagementViewModel
        {
            public int Id {get; set;}
            public string FirstName {get; set;}
            public string LastName {get; set;}
            public string Role {get; set;}
        }
    }
}
