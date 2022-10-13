namespace Temp.Domain.Models.Organizations.Exceptions;

public class InvalidOrganizationException : Exception
{
    public InvalidOrganizationException(string parameterName, object parameterValue)
        : base($"Invalid Employee, " +
             $"Parameter Name : {parameterName}, " +
             $"Parameter Value : {parameterValue}.") { }
}
