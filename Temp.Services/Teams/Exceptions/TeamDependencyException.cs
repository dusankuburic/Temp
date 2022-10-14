namespace Temp.Services.Teams.Exceptions;

public class TeamDependencyException : Exception
{
    public TeamDependencyException(Exception innerException)
        : base("Service dependency error occurred, contact support", innerException) {

    }
}

