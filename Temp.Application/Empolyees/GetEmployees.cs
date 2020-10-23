using System;
using System.Collections.Generic;
using System.Linq;

using System.Text;
using System.Threading.Tasks;
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

        private string RoleName(int EmpolyeeId)
        {
            var userTest = _ctx.Users
                .Where(x => x.Employee.Id == EmpolyeeId)
                .FirstOrDefault();

            if(userTest == null)
            {
               var adminTest = _ctx.Admins
                    .Where(x => x.Employee.Id == EmpolyeeId)
                    .FirstOrDefault();

                if(adminTest != null)
                {
                    return "Admin";
                }
            }
            else
            {
                return "User";
            }

            return "None";
        }



        public IEnumerable<EmployeeViewModel> Do() => 
            _ctx.Employees.ToList().Select(x => new EmployeeViewModel
            {
                Id = x.Id,
                FirstName = x.FirstName,
                LastName = x.LastName,
                Role = RoleName(x.Id)
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
