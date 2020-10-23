using System.Linq;
using Temp.Database;

namespace Temp.Application.Empolyees
{
    public class GetEmployee
    {
        private readonly ApplicationDbContext _ctx;

        public GetEmployee(ApplicationDbContext ctx)
        {
            _ctx = ctx;
        }

        public EmployeeViewModel Do(int id) => 
            _ctx.Employees.Where(x => x.Id == id).Select(x => new EmployeeViewModel
            {
                Id = x.Id,
                FirstName = x.FirstName,
                LastName = x.LastName
            })
            .FirstOrDefault();


        public class EmployeeViewModel
        {
            public int Id {get; set;}
            public string FirstName {get; set;}
            public string LastName {get; set;}
        }
    }
}
