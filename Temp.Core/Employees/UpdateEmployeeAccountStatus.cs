using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Temp.Database;

namespace Temp.Core.Employees;

public class UpdateEmployeeAccountStatus
{

    private readonly ApplicationDbContext _ctx;

    public UpdateEmployeeAccountStatus(ApplicationDbContext ctx) {
        _ctx = ctx;
    }

    public async Task<bool> Do(int EmployeeId) {
        Domain.Models.Employee employee = await _ctx.Employees
                .Include(x => x.User)
                .Include(x => x.Admin)
                .Include(x => x.Moderator)
                .Where(x => x.Id == EmployeeId)
                .FirstOrDefaultAsync();

        switch (employee.Role) {
            case "User":
                if (employee.User.IsActive) {
                    employee.User.IsActive = false;
                } else {
                    employee.User.IsActive = true;
                }

                break;
            case "Moderator":
                if (employee.Moderator.IsActive) {
                    employee.Moderator.IsActive = false;
                } else {
                    employee.Moderator.IsActive = true;
                }

                break;
            case "Admin":
                if (employee.Admin.IsActive) {
                    employee.Admin.IsActive = false;
                } else {
                    employee.Admin.IsActive = true;
                }

                break;
            case "None":
                break;
        }

        await _ctx.SaveChangesAsync();

        return true;
    }

}
