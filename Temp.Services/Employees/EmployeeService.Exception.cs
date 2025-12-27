using Temp.Services._Helpers;
using Temp.Services.Employees.Exceptions;
using Temp.Services.Employees.Models.Commands;
using Temp.Services.Employees.Models.Queries;

namespace Temp.Services.Employees;

public partial class EmployeeService
{
    private delegate Task<CreateEmployeeResponse> ReturningCreateEmployeeFunction();
    private delegate Task<PagedList<GetEmployeesResponse>> ReturningGetEmployeesFunction();
    private delegate Task<GetEmployeeResponse> ReturningGetEmployeeFunction();
    private delegate Task<UpdateEmployeeResponse> ReturningUpdateEmployeeFunction();
    private delegate Task<PagedList<GetEmployeesWithoutEngagementResponse>> ReturningEmployeesWithoutEngagement();
    private delegate Task<PagedList<GetEmployeesWithEngagementResponse>> ReturningEmployeesWithEngagement();
    private delegate Task<DeleteEmployeeResponse> ReturningDeleteEmployeeFunction();

    private async Task<CreateEmployeeResponse> TryCatch(ReturningCreateEmployeeFunction returningCreateEmployeeFunction) {
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

    private async Task<PagedList<GetEmployeesResponse>> TryCatch(ReturningGetEmployeesFunction returningGetEmployeesFunction) {
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

    private async Task<GetEmployeeResponse> TryCatch(ReturningGetEmployeeFunction returningGetEmployeeFunction) {
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

    private async Task<UpdateEmployeeResponse> TryCatch(ReturningUpdateEmployeeFunction returningUpdateEmployeeFunction) {
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

    private async Task<PagedList<GetEmployeesWithoutEngagementResponse>> TryCatch(ReturningEmployeesWithoutEngagement returningEmployeesWithoutEngagement) {
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

    private async Task<PagedList<GetEmployeesWithEngagementResponse>> TryCatch(ReturningEmployeesWithEngagement returningEmployeesWithEngagement) {
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

    private async Task<DeleteEmployeeResponse> TryCatch(ReturningDeleteEmployeeFunction returningDeleteEmployeeFunction) {
        try {
            return await returningDeleteEmployeeFunction();
        } catch (EmployeeNotFoundException employeeNotFoundException) {
            throw CreateAndLogValidationException(employeeNotFoundException);
        } catch (SqlException sqlException) {
            throw CreateAndLogCriticalDependencyException(sqlException);
        } catch (Exception exception) {
            throw CreateAndLogServiceException(exception);
        }
    }

    private EmployeeServiceException CreateAndLogServiceException(Exception exception) {
        var employeeServiceException = new EmployeeServiceException(exception);
        Logger.LogError(exception);
        Logger.LogError(employeeServiceException);
        return employeeServiceException;
    }

    private EmployeeValidationException CreateAndLogValidationException(Exception exception) {
        var employeeValidationException = new EmployeeValidationException(exception);
        Logger.LogError(employeeValidationException);
        return employeeValidationException;
    }

    private EmployeeDependencyException CreateAndLogCriticalDependencyException(Exception exception) {
        var employeeDependencyException = new EmployeeDependencyException(exception);
        Logger.LogCritical(employeeDependencyException);
        return employeeDependencyException;
    }
}