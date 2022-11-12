namespace Temp.Services.Employees.Exceptions;

public class InvalidEmployeeException : Exception
{
    public InvalidEmployeeException(string parameterName, object parameterValue)
        : base($"Invalid Employee, " +
              $"ParameterName : {parameterName}, " +
              $"ParameterValue: {parameterValue}.") { }
}
