﻿using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Temp.Domain.Models.Employees.Exceptions;

namespace Temp.Application.Employees
{
    public partial class EmployeeService
    {
        public delegate Task<CreateEmployee.Response> ReturningCreateEmployeeFunction();
        public delegate IEnumerable<GetEmployees.EmployeeViewModel> ReturningGetEmployeesFunction();
        public delegate GetEmployee.EmployeeViewModel ReturningGetEmployeeFunction();
        public delegate Task<UpdateEmployee.Response> ReturningUpdateEmployeeFunction();
        public delegate IEnumerable<GetEmployeesWithoutEngagement.EmployeesWithoutEngagementViewModel> ReturningEmployeesWithoutEngagement();
        public delegate IEnumerable<GetEmployeesWithEngagement.EmployeesWithEngagementViewModel> ReturningEmployeesWithEngagement();

        public async Task<CreateEmployee.Response> TryCatch(ReturningCreateEmployeeFunction returningCreateEmployeeFunction)
        {
            try
            {
                return await returningCreateEmployeeFunction();
            }
            catch(NullEmployeeException nullEmployeeException)
            {
                throw CreateAndLogValidationException(nullEmployeeException);
            }
            catch(InvalidEmployeeException invalidEmployeeException)
            {
                throw CreateAndLogValidationException(invalidEmployeeException);
            }
            catch(SqlException sqlException)
            {
                throw CreateAndLogCriticalDependencyException(sqlException);
            }
            catch(Exception exception)
            {
                throw CreateAndLogServiceException(exception);
            }
        }

        public IEnumerable<GetEmployees.EmployeeViewModel> TryCatch(ReturningGetEmployeesFunction returningGetEmployeesFunction)
        {
            try
            {
                return returningGetEmployeesFunction();
            }
            catch(EmployeeEmptyStorageException employeeEmptyStorageException) 
            {
                throw CreateAndLogValidationException(employeeEmptyStorageException);
            }
            catch(SqlException sqlException)
            {
                throw CreateAndLogCriticalDependencyException(sqlException);
            }
            catch(Exception exception)
            {
                throw CreateAndLogServiceException(exception);
            }
        }

        public GetEmployee.EmployeeViewModel TryCatch(ReturningGetEmployeeFunction returningGetEmployeeFunction)
        {
            try
            {
                return returningGetEmployeeFunction();
            }
            catch(NullEmployeeException nullEmployeeException)
            {
                throw CreateAndLogValidationException(nullEmployeeException);
            }
            catch(SqlException sqlException)
            {
                throw CreateAndLogCriticalDependencyException(sqlException);
            }
            catch(Exception exception)
            {
                throw CreateAndLogServiceException(exception);
            }
        }

        public async Task<UpdateEmployee.Response> TryCatch(ReturningUpdateEmployeeFunction returningUpdateEmployeeFunction)
        {
            try
            {
                return await returningUpdateEmployeeFunction();
            }
            catch(NullEmployeeException nullEmployeeException)
            {
                throw CreateAndLogValidationException(nullEmployeeException);
            }
            catch(InvalidEmployeeException invalidEmployeeException)
            {
                throw CreateAndLogValidationException(invalidEmployeeException);
            }
            catch(SqlException sqlException)
            {
                throw CreateAndLogCriticalDependencyException(sqlException);
            }
            catch(Exception exception)
            {
                throw CreateAndLogServiceException(exception);
            }
        }

        public IEnumerable<GetEmployeesWithoutEngagement.EmployeesWithoutEngagementViewModel> TryCatch(ReturningEmployeesWithoutEngagement returningEmployeesWithoutEngagement)
        {
            try
            {
                return returningEmployeesWithoutEngagement();
            }
            catch(EmployeeWithoutEngagementStorageException employeeWithoutEngagementStorageException) 
            {
                throw CreateAndLogValidationException(employeeWithoutEngagementStorageException);
            }
            catch(SqlException sqlException)
            {
                throw CreateAndLogCriticalDependencyException(sqlException);
            }
            catch(Exception exception)
            {
                throw CreateAndLogServiceException(exception);
            }
        }

        public IEnumerable<GetEmployeesWithEngagement.EmployeesWithEngagementViewModel> TryCatch(ReturningEmployeesWithEngagement returningEmployeesWithEngagement)
        {
            try
            {
                return returningEmployeesWithEngagement();
            }
            catch(EmployeeWithEngagementStorageException employeeWithEngagementStorageException) 
            {
                throw CreateAndLogValidationException(employeeWithEngagementStorageException);
            }
            catch(SqlException sqlException)
            {
                throw CreateAndLogCriticalDependencyException(sqlException);
            }
            catch(Exception exception)
            {
                throw CreateAndLogServiceException(exception);
            }
        }

        private EmployeeServiceException CreateAndLogServiceException(Exception exception)
        {
            var employeeServiceException = new EmployeeServiceException(exception);
            //LOG
            return employeeServiceException;
        }

        private EmployeeValidationException CreateAndLogValidationException(Exception exception)
        {
            var employeeValidationException = new EmployeeValidationException(exception);
            //LOG
            return employeeValidationException;
        }

        private EmployeeDependencyException CreateAndLogCriticalDependencyException(Exception exception)
        {
            var employeeDependencyException = new EmployeeDependencyException(exception);
            //LOG
            return employeeDependencyException;
        }
    }
}