namespace Temp.Services.Engagements.Exceptions;

public class EngagementDependencyException : Exception
{
    public EngagementDependencyException(Exception innerException)
        : base("Service dependency error occurred, contact support", innerException) { }
}
