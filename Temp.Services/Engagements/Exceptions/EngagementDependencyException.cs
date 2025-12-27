using Temp.Services.Exceptions;

namespace Temp.Services.Engagements.Exceptions;

public class EngagementDependencyException : DependencyException
{
    public EngagementDependencyException(Exception innerException)
        : base("Engagement service dependency error occurred", innerException) {
    }

    public EngagementDependencyException(string message, Exception innerException)
        : base(message, innerException) {
    }
}