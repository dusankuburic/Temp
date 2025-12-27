using ValidationEx = Temp.Services.Exceptions.ValidationException;

namespace Temp.Services.Groups.Exceptions;

public class NullGroupException : ValidationEx
{
    public NullGroupException()
        : base("Group cannot be null", "Group", "Group object is required") {
    }

    public NullGroupException(string fieldName)
        : base($"{fieldName} cannot be null", fieldName, "Value is required") {
    }
}