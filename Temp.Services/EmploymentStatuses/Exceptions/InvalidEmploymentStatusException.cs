namespace Temp.Services.EmploymentStatuses.Exceptions;

public class InvalidEmploymentStatusException : Exception
{
    public InvalidEmploymentStatusException(string parameterName, object parameterValue)
        : base($"Invalid Employee, " +
            $"Parameter Name: {parameterName}" +
            $"Parameter Value: {parameterValue}") {

    }
}
