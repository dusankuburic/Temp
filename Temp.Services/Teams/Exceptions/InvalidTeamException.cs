using ValidationEx = Temp.Services.Exceptions.ValidationException;

namespace Temp.Services.Teams.Exceptions;

public class InvalidTeamException : ValidationEx
{
    public InvalidTeamException(string parameterName, object parameterValue)
        : base($"Invalid team property: {parameterName}",
               parameterName,
               $"Invalid value: {parameterValue}") {
    }
}