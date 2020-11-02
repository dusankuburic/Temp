using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Temp.Domain.Models.Employees.Exceptions;

namespace Temp.Application.Empolyees
{
    public partial class EmployeeService
    {
        public delegate Task<CreateEmployee.Response> ReturningCreateEmployeeFunction();
        public delegate IEnumerable<GetEmployees.EmployeeViewModel> ReturningGetEmloyeesFunction();
        public delegate GetEmployee.EmployeeViewModel ReturningGetEmployeeFunction();
        public delegate Task<UpdateEmployee.Response> ReturningUpdateEmployeeFunction();

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
        }

        public IEnumerable<GetEmployees.EmployeeViewModel> TryCatch(ReturningGetEmloyeesFunction returningGetEmloyeesFunction)
        {
            try
            {
                return returningGetEmloyeesFunction();
            }
            catch(EmployeeEmptyStorageException employeeEmptyStorageException) 
            {
                throw CreateAndLogValidationException(employeeEmptyStorageException);
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
        }

 


        private EmployeeValidationException CreateAndLogValidationException(Exception exception)
        {
            var employeeValidationException = new EmployeeValidationException(exception);

            return employeeValidationException;
        }

    }
}
