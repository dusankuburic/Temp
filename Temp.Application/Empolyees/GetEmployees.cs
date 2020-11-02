using System.Collections.Generic;
using System.Linq;
using Temp.Database;

namespace Temp.Application.Empolyees
{
    public class GetEmployees
    {
        private readonly ApplicationDbContext _ctx;

        public GetEmployees(ApplicationDbContext ctx)
        {
            _ctx = ctx;
        }

        public IEnumerable<EmployeeViewModel> Do() => 
            _ctx.Employees.ToList()
            .Select(x => new EmployeeViewModel
            {
                Id = x.Id,
                FirstName = x.FirstName,
                LastName = x.LastName,
                Role = x.Role
            });

        public class EmployeeViewModel
        {
            public int Id {get; set;}
            public string FirstName {get; set;}
            public string LastName {get; set;}
            public string Role {get; set;}
        }
    }
}
