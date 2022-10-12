using System;

namespace Temp.Domain.Models.EmploymentStatuses.Exceptions;

public class EmploymentStatusDependencyException : Exception
{
    public EmploymentStatusDependencyException(Exception innerException)
        : base("Service dependency error occurred, contact support", innerException) {

    }
}
