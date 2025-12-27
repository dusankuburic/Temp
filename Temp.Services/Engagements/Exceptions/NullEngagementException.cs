using ValidationEx = Temp.Services.Exceptions.ValidationException;

namespace Temp.Services.Engagements.Exceptions;

public class NullEngagementException : ValidationEx
{
    public NullEngagementException()
        : base("Engagement cannot be null", "Engagement", "Engagement object is required") {
    }

    public NullEngagementException(string fieldName)
        : base($"{fieldName} cannot be null", fieldName, "Value is required") {
    }
}