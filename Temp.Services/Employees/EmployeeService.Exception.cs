﻿using Microsoft.Data.SqlClient;
using Temp.Services._Helpers;
using Temp.Services.Employees.Exceptions;
using Temp.Services.Employees.Models.Commands;
using Temp.Services.Employees.Models.Queries;

namespace Temp.Services.Employees;

public partial class EmployeeService
{
    public delegate Task<CreateEmployeeResponse> ReturningCreateEmployeeFunction();
    public delegate Task<PagedList<GetEmployeesResponse>> ReturningGetEmployeesFunction();
    public delegate Task<GetEmployeeResponse> ReturningGetEmployeeFunction();
    public delegate Task<UpdateEmployeeResponse> ReturningUpdateEmployeeFunction();
    public delegate Task<PagedList<GetEmployeesWithoutEngagementResponse>> ReturningEmployeesWithoutEngagement();
    public delegate Task<PagedList<GetEmployeesWithEngagementResponse>> ReturningEmployeesWithEngagement();

    public async Task<CreateEmployeeResponse> TryCatch(ReturningCreateEmployeeFunction returningCreateEmployeeFunction) {
        try {
            return await returningCreateEmployeeFunction();
        } catch (NullEmployeeException nullEmployeeException) {
            throw CreateAndLogValidationException(nullEmployeeException);
        } catch (InvalidEmployeeException invalidEmployeeException) {
            throw CreateAndLogValidationException(invalidEmployeeException);
        } catch (SqlException sqlException) {
            throw CreateAndLogCriticalDependencyException(sqlException);
        } catch (Exception exception) {
            throw CreateAndLogServiceException(exception);
        }
    }

    public async Task<PagedList<GetEmployeesResponse>> TryCatch(ReturningGetEmployeesFunction returningGetEmployeesFunction) {
        try {
            return await returningGetEmployeesFunction();
        } catch (EmployeeEmptyStorageException employeeEmptyStorageException) {
            throw CreateAndLogValidationException(employeeEmptyStorageException);
        } catch (SqlException sqlException) {
            throw CreateAndLogCriticalDependencyException(sqlException);
        } catch (Exception exception) {
            throw CreateAndLogServiceException(exception);
        }
    }

    public async Task<GetEmployeeResponse> TryCatch(ReturningGetEmployeeFunction returningGetEmployeeFunction) {
        try {
            return await returningGetEmployeeFunction();
        } catch (NullEmployeeException nullEmployeeException) {
            throw CreateAndLogValidationException(nullEmployeeException);
        } catch (SqlException sqlException) {
            throw CreateAndLogCriticalDependencyException(sqlException);
        } catch (Exception exception) {
            throw CreateAndLogServiceException(exception);
        }
    }

    public async Task<UpdateEmployeeResponse> TryCatch(ReturningUpdateEmployeeFunction returningUpdateEmployeeFunction) {
        try {
            return await returningUpdateEmployeeFunction();
        } catch (NullEmployeeException nullEmployeeException) {
            throw CreateAndLogValidationException(nullEmployeeException);
        } catch (InvalidEmployeeException invalidEmployeeException) {
            throw CreateAndLogValidationException(invalidEmployeeException);
        } catch (SqlException sqlException) {
            throw CreateAndLogCriticalDependencyException(sqlException);
        } catch (Exception exception) {
            throw CreateAndLogServiceException(exception);
        }
    }

    public async Task<PagedList<GetEmployeesWithoutEngagementResponse>> TryCatch(ReturningEmployeesWithoutEngagement returningEmployeesWithoutEngagement) {
        try {
            return await returningEmployeesWithoutEngagement();
        } catch (EmployeeWithoutEngagementStorageException employeeWithoutEngagementStorageException) {
            throw CreateAndLogValidationException(employeeWithoutEngagementStorageException);
        } catch (SqlException sqlException) {
            throw CreateAndLogCriticalDependencyException(sqlException);
        } catch (Exception exception) {
            throw CreateAndLogServiceException(exception);
        }
    }

    public async Task<PagedList<GetEmployeesWithEngagementResponse>> TryCatch(ReturningEmployeesWithEngagement returningEmployeesWithEngagement) {
        try {
            return await returningEmployeesWithEngagement();
        } catch (EmployeeWithEngagementStorageException employeeWithEngagementStorageException) {
            throw CreateAndLogValidationException(employeeWithEngagementStorageException);
        } catch (SqlException sqlException) {
            throw CreateAndLogCriticalDependencyException(sqlException);
        } catch (Exception exception) {
            throw CreateAndLogServiceException(exception);
        }
    }

    private EmployeeServiceException CreateAndLogServiceException(Exception exception) {
        var employeeServiceException = new EmployeeServiceException(exception);
        //LOG
        return employeeServiceException;
    }

    private EmployeeValidationException CreateAndLogValidationException(Exception exception) {
        var employeeValidationException = new EmployeeValidationException(exception);
        //LOG
        return employeeValidationException;
    }

    private EmployeeDependencyException CreateAndLogCriticalDependencyException(Exception exception) {
        var employeeDependencyException = new EmployeeDependencyException(exception);
        //LOG
        return employeeDependencyException;
    }
}

