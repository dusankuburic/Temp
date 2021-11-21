using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Temp.Database;

namespace Temp.Core.Employees
{
    public class GetEmployee : EmployeeService
    {
        private readonly ApplicationDbContext _ctx;

        public GetEmployee(ApplicationDbContext ctx) {
            _ctx = ctx;
        }

        public Task<EmployeeViewModel> Do(int id) =>
        TryCatch(async () => {
            var employee = await _ctx.Employees
                .Where(x => x.Id == id)
                .Select(x => new EmployeeViewModel
                {
                    Id = x.Id,
                    FirstName = x.FirstName,
                    LastName = x.LastName,
                    TeamId = x.TeamId,
                    Role = x.Role,

                })
                .FirstOrDefaultAsync();

            ValidateGetEmployeeViewModel(employee);

            return employee;
        });

        public class EmployeeViewModel
        {
            public int Id { get; set; }
            public string FirstName { get; set; }
            public string LastName { get; set; }
            public int? TeamId { get; set; }
            public string Role { get; set; }
        }
    }
}