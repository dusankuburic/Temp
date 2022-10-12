using System;

namespace Temp.Domain.Models.Employees.Exceptions;

public class EmployeeValidationException : Exception
{
    public EmployeeValidationException(Exception innerException)
        : base("Invalid input, contact support.", innerException) {

    }
}
