using Temp.Services.Exceptions;

namespace Temp.Services.Engagements.Exceptions;

public class EngagementServiceException : ServiceException
{
    public EngagementServiceException(Exception innerException)
        : base("Engagement service error occurred", "ENGAGEMENT_SERVICE_ERROR", innerException) {
    }

    public EngagementServiceException(string message, Exception innerException)
        : base(message, "ENGAGEMENT_SERVICE_ERROR", innerException) {
    }
}