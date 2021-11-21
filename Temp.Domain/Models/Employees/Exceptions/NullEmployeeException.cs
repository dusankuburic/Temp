using System;

namespace Temp.Domain.Models.Employees.Exceptions
{
    public class NullEmployeeException : Exception
    {
        public NullEmployeeException() : base("The employee is null.") { }
    }
}