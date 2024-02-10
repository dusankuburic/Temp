namespace Temp.Services.Engagements.Exceptions;

public class EngagementServiceException : Exception
{
    public EngagementServiceException(Exception innerException)
        : base("Service error, contact support", innerException) { }

}
