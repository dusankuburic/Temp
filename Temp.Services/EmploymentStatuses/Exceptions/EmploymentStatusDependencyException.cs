using Temp.Services.Exceptions;

namespace Temp.Services.EmploymentStatuses.Exceptions;

public class EmploymentStatusDependencyException : DependencyException
{
    public EmploymentStatusDependencyException(Exception innerException)
        : base("EmploymentStatus service dependency error occurred", innerException) {
    }

    public EmploymentStatusDependencyException(string message, Exception innerException)
        : base(message, innerException) {
    }
}