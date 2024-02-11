namespace Temp.Services.EmploymentStatuses.Exceptions;

public class NullEmploymentStatusException : Exception
{
    public NullEmploymentStatusException()
        : base("Employment Status is null") {

    }
}
