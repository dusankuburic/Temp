using System;

namespace Temp.Services.Employees.Exceptions
{
    public class EmployeeNotFoundException : Exception
    {
        public EmployeeNotFoundException() : base("The specified employee was not found.")
        {
        }

        public EmployeeNotFoundException(string message) : base(message)
        {
        }

        public EmployeeNotFoundException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}