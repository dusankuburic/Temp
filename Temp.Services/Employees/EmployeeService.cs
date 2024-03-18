using Temp.Database;
using Temp.Domain.Models;
using Temp.Services._Helpers;
using Temp.Services.Employees.Models.Commands;
using Temp.Services.Employees.Models.Queries;
using Temp.Services.Integrations.Loggings;
using Temp.Services.Providers;

namespace Temp.Services.Employees;

public partial class EmployeeService : IEmployeeService
{
    private readonly ApplicationDbContext _ctx;
    private readonly IMapper _mapper;
    private readonly ILoggingBroker _loggingBroker;
    private readonly IIdentityProvider _identityProvider;

    public EmployeeService(
        ApplicationDbContext ctx,
        IMapper mapper,
        ILoggingBroker loggingBroker,
        IIdentityProvider identityProvider) {
        _ctx = ctx;
        _mapper = mapper;
        _loggingBroker = loggingBroker;
        _identityProvider = identityProvider;
    }

    public Task<CreateEmployeeResponse> CreateEmployee(CreateEmployeeRequest request) =>
    TryCatch(async () => {
        var employee = _mapper.Map<Employee>(request);

        employee.SetAuditableInfoOnCreate(await _identityProvider.GetCurrentUser());

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

        employeesQuery = employeesQuery.OrderBy(x => x.FirstName)
            .AsQueryable();


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

        employeesWithEngagement = employeesWithEngagement.OrderBy(x => x.FirstName)
            .AsQueryable();

        var employees = await PagedList<GetEmployeesWithEngagementResponse>.CreateAsync(
            employeesWithEngagement,
            request.PageNumber,
            request.PageSize);

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

        employeesWithoutEngagement = employeesWithoutEngagement.OrderBy(x => x.FirstName)
            .AsQueryable();

        var employees = await PagedList<GetEmployeesWithoutEngagementResponse>.CreateAsync(
            employeesWithoutEngagement,
            request.PageNumber,
            request.PageSize);

        return employees;
    });

    public Task<UpdateEmployeeResponse> UpdateEmployee(UpdateEmployeeRequest request) =>
    TryCatch(async () => {
        var employee = await _ctx.Employees
            .Where(x => x.Id == request.Id)
            .FirstOrDefaultAsync();

        _mapper.Map(request, employee);

        employee.SetAuditableInfoOnUpdate(await _identityProvider.GetCurrentUser());

        ValidateEmployeeOnUpdate(employee);

        await _ctx.SaveChangesAsync();

        return new UpdateEmployeeResponse() {
            Success = true
        };
    });
}

