using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Temp.Core.Helpers;
using Temp.Database;

namespace Temp.Core.Employees
{
    public class GetEmployees : EmployeeService
    {
        private readonly ApplicationDbContext _ctx;

        public GetEmployees(ApplicationDbContext ctx) {
            _ctx = ctx;
        }

        public Task<PagedList<EmployeeViewModel>> Do(Request request) =>
        TryCatch(async () => {
            var employees = _ctx.Employees
            .Include(x => x.User)
            .Include(x => x.Moderator)
            .Include(x => x.Admin)
            .Select(x => new EmployeeViewModel
            {
                Id = x.Id,
                FirstName = x.FirstName,
                LastName = x.LastName,
                Role = x.Role,
                IsActive = x.User.IsActive || x.Moderator.IsActive || x.Admin.IsActive
            }).AsQueryable();

            if (!string.IsNullOrEmpty(request.Role)) {
                employees = employees.Where(x => x.Role == request.Role);
            }

            if (!string.IsNullOrEmpty(request.FirstName)) {
                employees = employees.Where(x => x.FirstName.Contains(request.FirstName));
            }

            if (!string.IsNullOrEmpty(request.LastName)) {
                employees = employees.Where(x => x.LastName.Contains(request.LastName));
            }


            ValidateStorageEmployees(employees);

            return await PagedList<EmployeeViewModel>.CreateAsync(employees, request.PageNumber, request.PageSize);
        });


        public class Request
        {
            private const int MaxPageSize = 20;
            public int PageNumber { get; set; } = 1;

            private int _pageSize = 10;

            public int PageSize
            {
                get { return _pageSize; }
                set { _pageSize = (value > MaxPageSize) ? MaxPageSize : value; }
            }

            public string Role { get; set; }

            public string FirstName { get; set; }
            public string LastName { get; set; }
        }

        public class EmployeeViewModel
        {
            public int Id { get; set; }
            public string FirstName { get; set; }
            public string LastName { get; set; }
            public string Role { get; set; }
            public bool IsActive { get; set; }
        }
    }
}