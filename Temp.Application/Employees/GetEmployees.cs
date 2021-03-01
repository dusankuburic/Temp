using System.Linq;
using System.Threading.Tasks;
using Temp.Application.Helpers;
using Temp.Database;


namespace Temp.Application.Employees
{
    public class GetEmployees : EmployeeService
    {
        private readonly ApplicationDbContext _ctx;

        public GetEmployees(ApplicationDbContext ctx)
        {
            _ctx = ctx;
        }

        public Task<PagedList<EmployeeViewModel>> Do(Request request) => 
        TryCatch(async() => 
        {
            var employees = _ctx.Employees
            .Select(x => new EmployeeViewModel
            {
                Id = x.Id,
                FirstName = x.FirstName,
                LastName = x.LastName,
                Role = x.Role
            }).AsQueryable();

            ValidateStorageEmployees(employees);

            return await PagedList<EmployeeViewModel>.CreateAsync(employees, request.PageNumber, request.PageSize);
        });


        public class Request
        {
            private const int MaxPageSize = 20;
            public int PageNumber { get; set; } = 1;

            private int pageSize = 10;

            public int PageSize
            {
                get { return pageSize; }
                set { pageSize = (value > MaxPageSize) ? MaxPageSize : value; }
            }
        }
        
        
        public class EmployeeViewModel
        {
            public int Id {get; set;}
            public string FirstName {get; set;}
            public string LastName {get; set;}
            public string Role {get; set;}
        }
    }
}
