namespace Temp.Services.EmploymentStatuses.Exceptions;

public class EmploymentStatusValidationException : Exception
{
    public EmploymentStatusValidationException(Exception innerException)
        : base("Invalid input, contact suppport", innerException) {

    }
}
