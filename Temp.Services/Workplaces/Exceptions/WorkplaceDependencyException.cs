using Temp.Services.Exceptions;

namespace Temp.Services.Workplaces.Exceptions;

public class WorkplaceDependencyException : DependencyException
{
    public WorkplaceDependencyException(Exception innerException)
        : base("Workplace service dependency error occurred", innerException) {
    }

    public WorkplaceDependencyException(string message, Exception innerException)
        : base(message, innerException) {
    }
}