using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using Temp.Core.Helpers;
using Temp.Domain.Models.Employees.Exceptions;

namespace Temp.Core.Employees
{
    public partial class EmployeeService
    {
        public delegate Task<CreateEmployee.Response> ReturningCreateEmployeeFunction();
        public delegate Task<PagedList<GetEmployees.EmployeeViewModel>> ReturningGetEmployeesFunction();
        public delegate Task<GetEmployee.EmployeeViewModel> ReturningGetEmployeeFunction();
        public delegate Task<UpdateEmployee.Response> ReturningUpdateEmployeeFunction();
        public delegate Task<PagedList<GetEmployeesWithoutEngagement.EmployeesWithoutEngagementViewModel>> ReturningEmployeesWithoutEngagement();
        public delegate Task<PagedList<GetEmployeesWithEngagement.EmployeesWithEngagementViewModel>> ReturningEmployeesWithEngagement();

        public async Task<CreateEmployee.Response> TryCatch(ReturningCreateEmployeeFunction returningCreateEmployeeFunction) {
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

        public async Task<PagedList<GetEmployees.EmployeeViewModel>> TryCatch(ReturningGetEmployeesFunction returningGetEmployeesFunction) {
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

        public async Task<GetEmployee.EmployeeViewModel> TryCatch(ReturningGetEmployeeFunction returningGetEmployeeFunction) {
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

        public async Task<UpdateEmployee.Response> TryCatch(ReturningUpdateEmployeeFunction returningUpdateEmployeeFunction) {
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

        public async Task<PagedList<GetEmployeesWithoutEngagement.EmployeesWithoutEngagementViewModel>> TryCatch(ReturningEmployeesWithoutEngagement returningEmployeesWithoutEngagement) {
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

        public async Task<PagedList<GetEmployeesWithEngagement.EmployeesWithEngagementViewModel>> TryCatch(ReturningEmployeesWithEngagement returningEmployeesWithEngagement) {
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
}