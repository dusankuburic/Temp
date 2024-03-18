namespace Temp.Services.Auth.Exceptions;

public class InvalidUserException : Exception
{

    public InvalidUserException(string parameterName, object parameterValue)
        : base(message: $"Invalid user, " +
            $"parameter name: {parameterName}, " +
            $"parameter value: {parameterValue}.") { }
}
