using Temp.Services._Helpers;
using Temp.Services.Employees.Models.Command;
using Temp.Services.Employees.Models.Query;

namespace Temp.Services.Employees;

public interface IEmployeeService
{
    Task<CreateEmployee.Response> CreateEmployee(CreateEmployee.Request request);

    Task<GetEmployee.EmployeeViewModel> GetEmployee(int id);

    Task<PagedList<GetEmployees.EmployeeViewModel>> GetEmployees(GetEmployees.Request request);

    Task<PagedList<GetEmployeesWithEngagement.EmployeesWithEngagementViewModel>> GetEmployeesWithEngagement(GetEmployeesWithEngagement.Request request);

    Task<PagedList<GetEmployeesWithoutEngagement.EmployeesWithoutEngagementViewModel>> GetEmployeesWithoutEngagement(GetEmployeesWithoutEngagement.Request request);

    Task<UpdateEmployee.Response> UpdateEmployee(int id, UpdateEmployee.Request request);

    Task<RemoveEmployeeRole.Response> RemoveEmployeeRole(RemoveEmployeeRole.Request request);

    Task<bool> UpdateEmployeeAccountStatus(int EmployeeId);

    Task<bool> UpdateEmployeeRole(string RoleName, int EmployeeId);

    Task<AssignRole.Response> AssignRole(AssignRole.Request request);
}

