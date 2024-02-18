using AutoMapper.QueryableExtensions;
using Temp.Database;
using Temp.Domain.Models;
using Temp.Services._Helpers;
using Temp.Services.Employees.Models.Commands;
using Temp.Services.Employees.Models.Queries;

namespace Temp.Services.Employees;

public partial class EmployeeService : IEmployeeService
{
    private readonly ApplicationDbContext _ctx;
    private readonly IMapper _mapper;

    public EmployeeService(ApplicationDbContext ctx, IMapper mapper) {
        _ctx = ctx;
        _mapper = mapper;
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

        ValidateStorageEmployees(employees);

        return employees;
    });

    public Task<PagedList<GetEmployeesWithEngagementResponse>>
    GetEmployeesWithEngagement(GetEmployeesWithEngagementRequest request) =>
    TryCatch(async () => {
        var currentDateTime = DateTime.Now;

        //TODO fix this horror
        var employeesWithEngagement = _ctx.Employees
            .Include(x => x.Engagements)
            .Where(x => x.Engagements.Count != 0)
            .Where(x => x.Engagements.Any(n => n.DateTo > currentDateTime))
            .OrderByDescending(x => x.Id)
            .Select(x => new GetEmployeesWithEngagementResponse
            {
                Id = x.Id,
                FirstName = x.FirstName,
                LastName = x.LastName,
                Role = x.Role,
                Salary = _ctx.Engagements
                    .Where(e => e.EmployeeId == x.Id)
                    .Select(e => e.Salary)
                    .ToList(),
                Workplace = _ctx.Engagements
                    .Where(e => e.EmployeeId == x.Id)
                    .Select(e => e.Workplace.Name)
                    .ToList(),
                EmploymentStatus = _ctx.Engagements
                    .Where(e => e.EmployeeId == x.Id)
                    .Select(e => e.EmploymentStatus.Name)
                    .ToList()
            })
            .AsQueryable();

        if (request.MinSalary != 0 || request.MaxSalary != 5000) {
            employeesWithEngagement = employeesWithEngagement
                .Where(x => x.Salary.All(sal => sal >= request.MinSalary && sal <= request.MaxSalary))
                .AsQueryable();
        }

        if (!string.IsNullOrEmpty(request.Workplace) && !string.IsNullOrEmpty(request.EmploymentStatus)) {
            employeesWithEngagement = employeesWithEngagement
                .Where(x =>
                    x.Workplace.Any(w => w.Contains(request.Workplace)) &&
                    x.EmploymentStatus.Any(e => e.Contains(request.EmploymentStatus)))
                .AsQueryable();
        } else if (!string.IsNullOrEmpty(request.Workplace)) {
            employeesWithEngagement = employeesWithEngagement
                .Where(x => x.Workplace.Any(w => w.Contains(request.Workplace)))
                .AsQueryable();

        } else if (!string.IsNullOrEmpty(request.EmploymentStatus)) {
            employeesWithEngagement = employeesWithEngagement
                .Where(x => x.EmploymentStatus.Any(es => es.Contains(request.EmploymentStatus)))
                .AsQueryable();
        }

        var employees = await PagedList<GetEmployeesWithEngagementResponse>.CreateAsync(
            employeesWithEngagement,
            request.PageNumber,
            request.PageSize);

        ValidateGetEmployeeWithEngagement(employees);

        return employees;
    });

    public Task<PagedList<GetEmployeesWithoutEngagementResponse>>
    GetEmployeesWithoutEngagement(GetEmployeesWithoutEngagementRequest request) =>
    TryCatch(async () => {

        var currentDateTime = DateTime.Now;

        var employeesWithoutEngagement = _ctx.Employees
            .Include(x => x.Engagements.Where(n => n.DateTo < currentDateTime))
            .OrderByDescending(x => x.Id)
            .ProjectTo<GetEmployeesWithoutEngagementResponse>(_mapper.ConfigurationProvider)
            .AsQueryable();

        var employees = await PagedList<GetEmployeesWithoutEngagementResponse>.CreateAsync(
            employeesWithoutEngagement,
            request.PageNumber,
            request.PageSize);

        ValidateGetEmployeeWithoutEngagement(employees);

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

    public Task<AssignRoleResponse> AssignRole(AssignRoleRequest request) {
        //if (request.Role == "User") {
        //    var userRequest = new RegisterUser.Request
        //        {
        //        Username = request.Username,
        //        Password = request.Password,
        //        EmployeeId = request.Id
        //    };

        //    var response = await new RegisterUser(_ctx).Do(userRequest);

        //    return new AssignRole.Response {
        //        Username = response.Username,
        //        Message = response.Messsage,
        //        Status = response.Status
        //    };

        //} else if (request.Role == "Admin") {
        //    var adminRequest = new RegisterAdmin.Request
        //        {
        //        Username = request.Username,
        //        Password = request.Password,
        //        EmployeeId = request.Id
        //    };

        //    var response = await new RegisterAdmin(_ctx).Do(adminRequest);

        //    return new AssignRole.Response {
        //        Username = response.Username,
        //        Message = response.Message,
        //        Status = response.Status
        //    };
        //} else if (request.Role == "Moderator") {
        //    var moderatorRequest = new RegisterModerator.Request
        //        {
        //        Username = request.Username,
        //        Password = request.Password,
        //        EmployeeId = request.Id
        //    };

        //    var response = await new RegisterModerator(_ctx).Do(moderatorRequest);

        //    return new AssignRole.Response {
        //        Username = response.Username,
        //        Message = response.Message,
        //        Status = response.Status
        //    };
        //} else {
        //    return new AssignRole.Response {
        //        Status = false,
        //        Message = "Wrong role!!!!"
        //    };
        //}
        return null;
    }
}

