using System.Linq;
using System.Threading.Tasks;
using Temp.Database;

namespace Temp.Application.Employees
{
    public class UpdateEmployeeRole
    {
        private readonly ApplicationDbContext _ctx;

        public UpdateEmployeeRole(ApplicationDbContext ctx)
        {
            _ctx = ctx;
        }

        public async Task<bool> Do(string RoleName, int EmployeeId)
        {
            var empolyee = _ctx.Employees.Where(x => x.Id == EmployeeId).First();    
            empolyee.Role = RoleName;
            await _ctx.SaveChangesAsync();

            if(empolyee.Role != RoleName)
            {
               return false;
            }

            return true;    
        }
    }
}
