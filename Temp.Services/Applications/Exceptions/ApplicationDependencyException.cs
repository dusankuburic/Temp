using Temp.Services.Exceptions;

namespace Temp.Services.Applications.Exceptions;

public class ApplicationDependencyException : DependencyException
{
    public ApplicationDependencyException(Exception innerException)
        : base("Application service dependency error occurred", innerException) {
    }

    public ApplicationDependencyException(string message, Exception innerException)
        : base(message, innerException) {
    }
}