using System;

namespace Temp.Domain.Models.EmploymentStatuses.Exceptions;

public class EmploymentStatusValidationException : Exception
{
    public EmploymentStatusValidationException(Exception innerException)
        : base("Invalid input, contact support", innerException) {

    }
}
