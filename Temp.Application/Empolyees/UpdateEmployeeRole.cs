using System.Linq;
using System.Threading.Tasks;
using Temp.Database;

namespace Temp.Application.Empolyees
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
            var empolyee = _ctx.Employees.FirstOrDefault(x => x.Id == EmployeeId);    
            empolyee.Role = RoleName;
            await _ctx.SaveChangesAsync();

            return true;    
        }
    }
}
