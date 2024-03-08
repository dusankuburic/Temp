using Temp.Database;
using Temp.Domain.Models;
using Temp.Services._Helpers;
using Temp.Services.Employees.Models.Commands;
using Temp.Services.Employees.Models.Queries;
using Temp.Services.Integrations.Loggings;

namespace Temp.Services.Employees;

public partial class EmployeeService : IEmployeeService
{
    private readonly ApplicationDbContext _ctx;
    private readonly IMapper _mapper;
    private readonly ILoggingBroker _loggingBroker;

    public EmployeeService(
        ApplicationDbContext ctx,
        IMapper mapper,
        ILoggingBroker loggingBroker) {
        _ctx = ctx;
        _mapper = mapper;
        _loggingBroker = loggingBroker;
    }

    public Task<CreateEmployeeResponse> CreateEmployee(CreateEmployeeRequest request) =>
    TryCatch(async () => {
        var employee = _mapper.Map<Employee>(request);

        ValidateEmployeeOnCreate(employee);

        _ctx.Employees.Add(employee);
        await _ctx.SaveChangesAsync();

        return _mapper.Map<CreateEmployeeResponse>(employee);
    });

    public Task<GetEmployeeResponse> GetEmployee(int id) =>
    TryCatch(async () => {
        var employee = await _ctx.Employees
            .Where(x => x.Id == id)
            .ProjectTo<GetEmployeeResponse>(_mapper.ConfigurationProvider)
            .FirstOrDefaultAsync();

        ValidateGetEmployee(employee);

        return employee;
    });

    public Task<PagedList<GetEmployeesResponse>> GetEmployees(GetEmployeesRequest request) =>
    TryCatch(async () => {
        var employeesQuery = _ctx.Employees
            .Include(x => x.User)
            .Include(x => x.Moderator)
            .Include(x => x.Admin)
            .ProjectTo<GetEmployeesResponse>(_mapper.ConfigurationProvider)
            .AsNoTracking()
            .AsQueryable();

        if (!string.IsNullOrEmpty(request.Role)) {
            employeesQuery = employeesQuery.Where(x => x.Role == request.Role)
                .AsQueryable();
        }

        if (!string.IsNullOrEmpty(request.FirstName)) {
            employeesQuery = employeesQuery.Where(x => x.FirstName.Contains(request.FirstName))
                .AsQueryable();
        }

        if (!string.IsNullOrEmpty(request.LastName)) {
            employeesQuery = employeesQuery.Where(x => x.LastName.Contains(request.LastName))
                .AsQueryable();
        }

        var employees = await PagedList<GetEmployeesResponse>.CreateAsync(
            employeesQuery,
            request.PageNumber,
            request.PageSize);

        return employees;
    });

    public Task<PagedList<GetEmployeesWithEngagementResponse>>
    GetEmployeesWithEngagement(GetEmployeesWithEngagementRequest request) =>
    TryCatch(async () => {

        var employeesWithEngagement = _ctx.Employees
            .Include(x => x.Engagements.Where(n => n.DateTo > DateTime.UtcNow))
            .Where(x => x.Engagements.Count > 0)
            .OrderByDescending(x => x.Id)
            .ProjectTo<GetEmployeesWithEngagementResponse>(_mapper.ConfigurationProvider)
            .AsQueryable();

        if (!string.IsNullOrEmpty(request.Role)) {
            employeesWithEngagement = employeesWithEngagement.Where(x => x.Role == request.Role)
                .AsQueryable();
        }

        if (!string.IsNullOrEmpty(request.FirstName)) {
            employeesWithEngagement = employeesWithEngagement.Where(x => x.FirstName.Contains(request.FirstName))
                .AsQueryable();
        }

        if (!string.IsNullOrEmpty(request.LastName)) {
            employeesWithEngagement = employeesWithEngagement.Where(x => x.LastName.Contains(request.LastName))
                .AsQueryable();
        }

        var employees = await PagedList<GetEmployeesWithEngagementResponse>.CreateAsync(
            employeesWithEngagement,
            request.PageNumber,
            request.PageSize);

        //ValidateGetEmployeeWithEngagement(employees);

        return employees;
    });

    public Task<PagedList<GetEmployeesWithoutEngagementResponse>>
    GetEmployeesWithoutEngagement(GetEmployeesWithoutEngagementRequest request) =>
    TryCatch(async () => {

        var currentDateTime = DateTime.UtcNow;

        var employeesWithoutEngagement = _ctx.Employees
            .Include(x => x.Engagements.Where(n => n.DateTo < currentDateTime))
            .OrderByDescending(x => x.Id)
            .ProjectTo<GetEmployeesWithoutEngagementResponse>(_mapper.ConfigurationProvider)
            .AsQueryable();

        if (!string.IsNullOrEmpty(request.Role)) {
            employeesWithoutEngagement = employeesWithoutEngagement.Where(x => x.Role == request.Role)
               .AsQueryable();
        }

        if (!string.IsNullOrEmpty(request.FirstName)) {
            employeesWithoutEngagement = employeesWithoutEngagement.Where(x => x.FirstName.Contains(request.FirstName))
                .AsQueryable();
        }

        if (!string.IsNullOrEmpty(request.LastName)) {
            employeesWithoutEngagement = employeesWithoutEngagement.Where(x => x.LastName.Contains(request.LastName))
                .AsQueryable();
        }

        var employees = await PagedList<GetEmployeesWithoutEngagementResponse>.CreateAsync(
            employeesWithoutEngagement,
            request.PageNumber,
            request.PageSize);

        //ValidateGetEmployeeWithoutEngagement(employees);

        return employees;
    });

    public Task<UpdateEmployeeResponse> UpdateEmployee(UpdateEmployeeRequest request) =>
    TryCatch(async () => {
        var employee = await _ctx.Employees
            .Where(x => x.Id == request.Id)
            .FirstOrDefaultAsync();

        _mapper.Map(request, employee);

        ValidateEmployeeOnUpdate(employee);

        await _ctx.SaveChangesAsync();

        return new UpdateEmployeeResponse() {
            Success = true
        };
    });

    public async Task<RemoveEmployeeRoleResponse> RemoveEmployeeRole(RemoveEmployeeRoleRequest request) {
        var employee = _ctx.Employees.FirstOrDefault(x => x.Id == request.Id);

        if (employee.Role == "Admin") {
            var admin = await _ctx.Admins.FirstOrDefaultAsync(x => x.EmployeeId == request.Id);
            _ctx.Admins.Remove(admin);
        } else if (employee.Role == "User") {
            var user = await _ctx.Users.FirstOrDefaultAsync(x => x.EmployeeId == request.Id);
            _ctx.Users.Remove(user);
        } else if (employee.Role == "Moderator") {
            var moderator = await _ctx.Moderators.FirstOrDefaultAsync(x => x.EmployeeId == request.Id);
            _ctx.Moderators.Remove(moderator);
        } else {
            employee.Role = "None";
        }
        await _ctx.SaveChangesAsync();

        return new RemoveEmployeeRoleResponse();
    }

    public async Task<bool> UpdateEmployeeAccountStatus(int EmployeeId) {
        var employee = await _ctx.Employees
            .Include(x => x.User)
            .Include(x => x.Admin)
            .Include(x => x.Moderator)
            .Where(x => x.Id == EmployeeId)
            .AsNoTracking()
            .FirstOrDefaultAsync();

        switch (employee.Role) {
            case "User":
                employee.User.IsActive = !employee.User.IsActive;
                break;
            case "Moderator":
                employee.Moderator.IsActive = !employee.Moderator.IsActive;
                break;
            case "Admin":
                employee.Admin.IsActive = !employee.Admin.IsActive;
                break;
            case "None":
                break;
        }

        await _ctx.SaveChangesAsync();

        return true;
    }

    public async Task<bool> UpdateEmployeeRole(string RoleName, int EmployeeId) {
        var empolyee = await _ctx.Employees
            .Where(x => x.Id == EmployeeId)
            .FirstOrDefaultAsync();

        empolyee.Role = RoleName;
        await _ctx.SaveChangesAsync();

        return empolyee.Role == RoleName;
    }


}

