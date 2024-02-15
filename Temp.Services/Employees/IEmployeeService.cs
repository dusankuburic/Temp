using Temp.Services._Helpers;
using Temp.Services.Employees.Models.Commands;
using Temp.Services.Employees.Models.Queries;

namespace Temp.Services.Employees;

public interface IEmployeeService
{
    Task<CreateEmployeeResponse> CreateEmployee(CreateEmployeeRequest request);

    Task<GetEmployeeResponse> GetEmployee(int id);

    Task<PagedList<GetEmployeesResponse>> GetEmployees(GetEmployeesRequest request);

    Task<PagedList<GetEmployeesWithEngagementResponse>> GetEmployeesWithEngagement(GetEmployeesWithEngagementRequest request);

    Task<PagedList<GetEmployeesWithoutEngagementResponse>> GetEmployeesWithoutEngagement(GetEmployeesWithoutEngagementRequest request);

    Task<UpdateEmployeeResponse> UpdateEmployee(UpdateEmployeeRequest request);

    Task<RemoveEmployeeRoleResponse> RemoveEmployeeRole(RemoveEmployeeRoleRequest request);

    Task<bool> UpdateEmployeeAccountStatus(int EmployeeId);

    Task<bool> UpdateEmployeeRole(string RoleName, int EmployeeId);

    Task<AssignRoleResponse> AssignRole(AssignRoleRequest request);
}

