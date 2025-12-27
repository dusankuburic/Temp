using ValidationEx = Temp.Services.Exceptions.ValidationException;

namespace Temp.Services.Groups.Exceptions;

public class NullGroupInnerTeamsException : ValidationEx
{
    public NullGroupInnerTeamsException()
        : base("Inner Team cannot be null", "InnerTeam", "Inner Team object is required") {
    }

    public NullGroupInnerTeamsException(string fieldName)
        : base($"{fieldName} cannot be null", fieldName, "Value is required") {
    }
}