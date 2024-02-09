﻿using Temp.Domain.Models;
using Temp.Services.Employees.Exceptions;
using Temp.Services.Employees.Models.Query;

namespace Temp.Services.Employees;

public partial class EmployeeService
{
    public void ValidateEmployeeOnCreate(Employee employee) {
        ValidateEmployee(employee);
        ValidateEmployeeStrings(employee);
    }

    public void ValidateEmployeeOnUpdate(Employee employee) {
        ValidateEmployee(employee);
        ValidateEmployeeStrings(employee);
    }

    public void ValidateEmployee(Employee employee) {
        if (employee is null) {
            throw new NullEmployeeException();
        }
    }

    public void ValidateGetEmployeeViewModel(GetEmployee.EmployeeViewModel employee) {
        if (employee is null) {
            throw new NullEmployeeException();
        }
    }

    public void ValidateGetEmployeeWithoutEngagementViewModel
        (IQueryable<GetEmployeesWithoutEngagement.EmployeesWithoutEngagementViewModel> employeeWithoutEngagement) {
        if (employeeWithoutEngagement.Count() == 0) {
            throw new EmployeeWithoutEngagementStorageException();
        }
    }

    public void ValidateGetEmployeeWithEngagementViewModel
        (IQueryable<GetEmployeesWithEngagement.EmployeesWithEngagementViewModel> employeeWithEngagement) {
        if (employeeWithEngagement.Count() == 0) {
            throw new EmployeeWithEngagementStorageException();
        }
    }

    public void ValidateEmployeeStrings(Employee employee) {
        switch (employee) {
            case { } when IsInvalid(employee.FirstName):
                throw new InvalidEmployeeException(
                    parameterName: nameof(employee.FirstName),
                    parameterValue: employee.FirstName);
            case { } when IsInvalid(employee.LastName):
                throw new InvalidEmployeeException(
                    parameterName: nameof(employee.LastName),
                    parameterValue: employee.LastName);
        }
    }

    public void ValidateStorageEmployees(IQueryable<GetEmployees.EmployeeViewModel> storageEmployees) {
        if (storageEmployees.Count() == 0) {
            throw new EmployeeEmptyStorageException();
        }
    }

    public static bool IsInvalid(string input) {
        return string.IsNullOrWhiteSpace(input);
    }
}
