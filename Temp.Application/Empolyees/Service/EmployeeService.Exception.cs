using System;
using System.Threading.Tasks;
using Temp.Domain.Models.Employees.Exceptions;

namespace Temp.Application.Empolyees
{
    public partial class EmployeeService
    {
        public delegate Task<CreateEmployee.Response> ReturningStudentFunction();

        public async Task<CreateEmployee.Response> TryCatch(ReturningStudentFunction returningStudentFunction)
        {
            try
            {
                return await returningStudentFunction();
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
