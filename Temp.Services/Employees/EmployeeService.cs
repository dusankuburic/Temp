using Temp.Database.UnitOfWork;
using Temp.Domain.Models;
using Temp.Services._Helpers;
using Temp.Services.Employees.Models.Commands;
using Temp.Services.Employees.Models.Queries;
using Temp.Services.Integrations.Loggings;
using Temp.Services.Providers;
using Temp.Services.Employees.Exceptions;

namespace Temp.Services.Employees;

public partial class EmployeeService : IEmployeeService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ILoggingBroker _loggingBroker;
    private readonly IIdentityProvider _identityProvider;

    public EmployeeService(
        IUnitOfWork unitOfWork,
        IMapper mapper,
        ILoggingBroker loggingBroker,
        IIdentityProvider identityProvider) {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _loggingBroker = loggingBroker;
        _identityProvider = identityProvider;
    }

    public Task<CreateEmployeeResponse> CreateEmployee(CreateEmployeeRequest request) =>
    TryCatch(async () => {
        var employee = _mapper.Map<Employee>(request);

        employee.SetAuditableInfoOnCreate(await _identityProvider.GetCurrentUser());

        ValidateEmployeeOnCreate(employee);

        await _unitOfWork.Employees.AddAsync(employee);
        await _unitOfWork.SaveChangesAsync();

        return _mapper.Map<CreateEmployeeResponse>(employee);
    });

    public Task<GetEmployeeResponse> GetEmployee(int id) =>
    TryCatch(async () => {
        var employee = await _unitOfWork.Employees
            .QueryNoTracking()
            .Where(x => x.Id == id)
            .ProjectTo<GetEmployeeResponse>(_mapper.ConfigurationProvider)
            .FirstOrDefaultAsync();

        ValidateGetEmployee(employee);

        return employee;
    });

    public Task<PagedList<GetEmployeesResponse>> GetEmployees(GetEmployeesRequest request) =>
    TryCatch(async () => {
        var employeesQuery = _unitOfWork.Employees
            .QueryNoTracking()
            .ProjectTo<GetEmployeesResponse>(_mapper.ConfigurationProvider);

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

        var employeesWithEngagement = _unitOfWork.Employees
            .Query()
            .Include(x => x.Engagements.Where(n => n.DateTo > DateTime.UtcNow))
            .Where(x => x.Engagements.Count > 0)
            .OrderByDescending(x => x.Id)
            .ProjectTo<GetEmployeesWithEngagementResponse>(_mapper.ConfigurationProvider);

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

        var employeesWithoutEngagement = _unitOfWork.Employees
            .Query()
            .Include(x => x.Engagements.Where(n => n.DateTo < currentDateTime))
            .OrderByDescending(x => x.Id)
            .ProjectTo<GetEmployeesWithoutEngagementResponse>(_mapper.ConfigurationProvider);

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
        var employee = await _unitOfWork.Employees
            .FirstOrDefaultAsync(x => x.Id == request.Id);

        _mapper.Map(request, employee);

        employee.SetAuditableInfoOnUpdate(await _identityProvider.GetCurrentUser());

        ValidateEmployeeOnUpdate(employee);

        _unitOfWork.Employees.Update(employee);
        await _unitOfWork.SaveChangesAsync();

        return new UpdateEmployeeResponse() {
            Success = true
        };
    });

    public async Task<DeleteEmployeeResponse> DeleteEmployeeAsync(int id)
    {
        return await TryCatch(async () =>
        {
            var employee = await _unitOfWork.Employees.GetByIdAsync(id);

            if (employee == null)
            {
                throw new EmployeeNotFoundException();
            }

            _unitOfWork.Employees.Remove(employee);
            await _unitOfWork.SaveChangesAsync();

            return new DeleteEmployeeResponse { Success = true };
        });
    }
}

