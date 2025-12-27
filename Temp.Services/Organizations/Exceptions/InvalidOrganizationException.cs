using ValidationEx = Temp.Services.Exceptions.ValidationException;

namespace Temp.Services.Organizations.Exceptions;

public class InvalidOrganizationException : ValidationEx
{
    public InvalidOrganizationException(string parameterName, object parameterValue)
        : base($"Invalid organization property: {parameterName}",
               parameterName,
               $"Invalid value: {parameterValue}") {
    }
}