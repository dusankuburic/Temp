using System;

namespace Temp.Domain.Models.Employees.Exceptions
{
    public class StudentDependencyException : Exception
    {
        public StudentDependencyException(Exception innerException)
            : base("Service dependency error occurred, contact support", innerException)
        {

        }
    }
}
