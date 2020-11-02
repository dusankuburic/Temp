using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Temp.Database;

namespace Temp.Application.Empolyees
{
    public class GetEmployees : EmployeeService
    {
        private readonly ApplicationDbContext _ctx;

        public GetEmployees(ApplicationDbContext ctx)
        {
            _ctx = ctx;
        }

        public IEnumerable<EmployeeViewModel> Do() => 
        TryCatch(() => 
        {
            var employees = _ctx.Employees.ToList()
            .Select(x => new EmployeeViewModel
            {
                Id = x.Id,
                FirstName = x.FirstName,
                LastName = x.LastName,
                Role = x.Role
            });


            ValidateStorageEmployees(employees);

            return employees;
            
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
