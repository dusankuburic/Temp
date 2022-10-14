namespace Temp.Services.Teams.Exceptions;

public class TeamServiceException : Exception
{
    public TeamServiceException(Exception innerException)
        : base("Service error, contact support", innerException) {

    }
}

