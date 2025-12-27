using Temp.Database.UnitOfWork;
using Temp.Domain.Models;
using Temp.Services._Helpers;
using Temp.Services._Shared;
using Temp.Services.Employees.Exceptions;
using Temp.Services.Employees.Models.Commands;
using Temp.Services.Employees.Models.Queries;
using Temp.Services.Integrations.Loggings;
using Temp.Services.Providers;

namespace Temp.Services.Employees;

public partial class EmployeeService : BaseService<Employee>, IEmployeeService
{
    public EmployeeService(
        IUnitOfWork unitOfWork,
        IMapper mapper,
        ILoggingBroker loggingBroker,
        IIdentityProvider identityProvider)
        : base(unitOfWork, mapper, loggingBroker, identityProvider) {
    }

    public Task<CreateEmployeeResponse> CreateEmployee(CreateEmployeeRequest request) =>
    TryCatch(async () => {
        var employee = Mapper.Map<Employee>(request);

        employee.SetAuditableInfoOnCreate(await IdentityProvider.GetCurrentUser());

        ValidateEmployeeOnCreate(employee);

        await UnitOfWork.Employees.AddAsync(employee);
        await UnitOfWork.SaveChangesAsync();

        return Mapper.Map<CreateEmployeeResponse>(employee);
    });

    public Task<GetEmployeeResponse> GetEmployee(int id) =>
    TryCatch(async () => {
        var employee = await UnitOfWork.Employees
            .QueryNoTracking()
            .Where(x => x.Id == id)
            .ProjectTo<GetEmployeeResponse>(Mapper.ConfigurationProvider)
            .FirstOrDefaultAsync();

        ValidateGetEmployee(employee);

        return employee;
    });

    public Task<PagedList<GetEmployeesResponse>> GetEmployees(GetEmployeesRequest request) =>
    TryCatch(async () => {
        var employeesQuery = UnitOfWork.Employees
            .QueryNoTracking()
            .ProjectTo<GetEmployeesResponse>(Mapper.ConfigurationProvider);

        if (!string.IsNullOrEmpty(request.Role)) {
            employeesQuery = employeesQuery.Where(x => x.Role == request.Role);
        }

        if (!string.IsNullOrEmpty(request.FirstName)) {
            employeesQuery = employeesQuery.Where(x => x.FirstName.Contains(request.FirstName));
        }

        if (!string.IsNullOrEmpty(request.LastName)) {
            employeesQuery = employeesQuery.Where(x => x.LastName.Contains(request.LastName));
        }

        employeesQuery = employeesQuery.OrderBy(x => x.FirstName);

        var employees = await PagedList<GetEmployeesResponse>.CreateAsync(
            employeesQuery,
            request.PageNumber,
            request.PageSize);

        return employees;
    });

    public Task<PagedList<GetEmployeesWithEngagementResponse>>
    GetEmployeesWithEngagement(GetEmployeesWithEngagementRequest request) =>
    TryCatch(async () => {


        var employeesWithEngagement = UnitOfWork.Employees
            .QueryNoTracking()
            .Where(x => x.Engagements.Any(n => n.DateTo > DateTime.UtcNow))
            .OrderByDescending(x => x.Id)
            .ProjectTo<GetEmployeesWithEngagementResponse>(Mapper.ConfigurationProvider);

        if (!string.IsNullOrEmpty(request.Role)) {
            employeesWithEngagement = employeesWithEngagement.Where(x => x.Role == request.Role);
        }

        if (!string.IsNullOrEmpty(request.FirstName)) {
            employeesWithEngagement = employeesWithEngagement.Where(x => x.FirstName.Contains(request.FirstName));
        }

        if (!string.IsNullOrEmpty(request.LastName)) {
            employeesWithEngagement = employeesWithEngagement.Where(x => x.LastName.Contains(request.LastName));
        }

        employeesWithEngagement = employeesWithEngagement.OrderBy(x => x.FirstName);

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


        var employeesWithoutEngagement = UnitOfWork.Employees
            .QueryNoTracking()
            .Where(x => !x.Engagements.Any(n => n.DateTo > currentDateTime))
            .OrderByDescending(x => x.Id)
            .ProjectTo<GetEmployeesWithoutEngagementResponse>(Mapper.ConfigurationProvider);

        if (!string.IsNullOrEmpty(request.Role)) {
            employeesWithoutEngagement = employeesWithoutEngagement.Where(x => x.Role == request.Role);
        }

        if (!string.IsNullOrEmpty(request.FirstName)) {
            employeesWithoutEngagement = employeesWithoutEngagement.Where(x => x.FirstName.Contains(request.FirstName));
        }

        if (!string.IsNullOrEmpty(request.LastName)) {
            employeesWithoutEngagement = employeesWithoutEngagement.Where(x => x.LastName.Contains(request.LastName));
        }

        employeesWithoutEngagement = employeesWithoutEngagement.OrderBy(x => x.FirstName);

        var employees = await PagedList<GetEmployeesWithoutEngagementResponse>.CreateAsync(
            employeesWithoutEngagement,
            request.PageNumber,
            request.PageSize);

        return employees;
    });

    public Task<UpdateEmployeeResponse> UpdateEmployee(UpdateEmployeeRequest request) =>
    TryCatch(async () => {
        var employee = await UnitOfWork.Employees
            .FirstOrDefaultAsync(x => x.Id == request.Id);

        Mapper.Map(request, employee);

        employee.SetAuditableInfoOnUpdate(await IdentityProvider.GetCurrentUser());

        ValidateEmployeeOnUpdate(employee);

        UnitOfWork.Employees.Update(employee);
        await UnitOfWork.SaveChangesAsync();

        return new UpdateEmployeeResponse() {
            Success = true
        };
    });

    public async Task<DeleteEmployeeResponse> DeleteEmployeeAsync(int id) {
        return await TryCatch(async () => {
            var employee = await UnitOfWork.Employees.GetByIdAsync(id);

            if (employee == null) {
                throw new EmployeeNotFoundException();
            }

            UnitOfWork.Employees.Remove(employee);
            await UnitOfWork.SaveChangesAsync();

            return new DeleteEmployeeResponse { Success = true };
        });
    }
}