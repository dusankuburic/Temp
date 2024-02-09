namespace Temp.Services.Workplaces.Exceptions;

public class InvalidWorkplaceException : Exception
{
    public InvalidWorkplaceException(string parameterName, object parameterValue)
        : base($"Invalid Workplace, " +
            $"Parameter Name: {parameterName}" +
            $"Parameter Value: {parameterValue}.") { }
}
