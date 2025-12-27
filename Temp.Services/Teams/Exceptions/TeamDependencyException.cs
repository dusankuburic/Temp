using Temp.Services.Exceptions;

namespace Temp.Services.Teams.Exceptions;

public class TeamDependencyException : DependencyException
{
    public TeamDependencyException(Exception innerException)
        : base("Team service dependency error occurred", innerException) {
    }

    public TeamDependencyException(string message, Exception innerException)
        : base(message, innerException) {
    }
}