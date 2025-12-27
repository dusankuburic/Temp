using ValidationEx = Temp.Services.Exceptions.ValidationException;

namespace Temp.Services.EmploymentStatuses.Exceptions;

public class NullEmploymentStatusException : ValidationEx
{
    public NullEmploymentStatusException()
        : base("EmploymentStatus cannot be null", "EmploymentStatus", "EmploymentStatus object is required") {
    }

    public NullEmploymentStatusException(string fieldName)
        : base($"{fieldName} cannot be null", fieldName, "Value is required") {
    }
}