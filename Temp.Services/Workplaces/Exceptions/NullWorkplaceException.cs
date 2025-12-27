using ValidationEx = Temp.Services.Exceptions.ValidationException;

namespace Temp.Services.Workplaces.Exceptions;

public class NullWorkplaceException : ValidationEx
{
    public NullWorkplaceException()
        : base("Workplace cannot be null", "Workplace", "Workplace object is required") {
    }

    public NullWorkplaceException(string fieldName)
        : base($"{fieldName} cannot be null", fieldName, "Value is required") {
    }
}