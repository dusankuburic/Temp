using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
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

        public IEnumerable<EmployeesWithoutEngagementViewModel> Do() =>
        TryCatch(() =>
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
            })
            .ToList();

            ValidateGetEmployeeWithoutEngagementViewModel(employeesWithoutEngagement);

            return employeesWithoutEngagement;
        });



        public class EmployeesWithoutEngagementViewModel
        {
            public int Id {get; set;}
            public string FirstName {get; set;}
            public string LastName {get; set;}
            public string Role {get; set;}
        }
    }
}
