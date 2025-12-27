using Temp.Services.Exceptions;

namespace Temp.Services.Teams.Exceptions;

public class TeamServiceException : ServiceException
{
    public TeamServiceException(Exception innerException)
        : base("Team service error occurred", "TEAM_SERVICE_ERROR", innerException) {
    }

    public TeamServiceException(string message, Exception innerException)
        : base(message, "TEAM_SERVICE_ERROR", innerException) {
    }
}