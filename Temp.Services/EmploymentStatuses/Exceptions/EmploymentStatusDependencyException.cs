namespace Temp.Services.EmploymentStatuses.Exceptions;

public class EmploymentStatusDependencyException : Exception
{
    public EmploymentStatusDependencyException(Exception innerException)
        : base("Service dependency error occurred, contaxt support", innerException) {

    }
}
