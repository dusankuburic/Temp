using System;

namespace Temp.Domain.Models.Employees.Exceptions
{
    public class EmployeeEmptyStorageException : Exception
    {
        public EmployeeEmptyStorageException() : base("No employees found in storage")
        {

        }
    }
}
