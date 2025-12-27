using ValidationEx = Temp.Services.Exceptions.ValidationException;

namespace Temp.Services.Teams.Exceptions;

public class NullTeamException : ValidationEx
{
    public NullTeamException()
        : base("Team cannot be null", "Team", "Team object is required") {
    }

    public NullTeamException(string fieldName)
        : base($"{fieldName} cannot be null", fieldName, "Value is required") {
    }
}