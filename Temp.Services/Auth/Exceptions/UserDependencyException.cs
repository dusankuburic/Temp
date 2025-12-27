using Temp.Services.Exceptions;

namespace Temp.Services.Auth.Exceptions;

public class UserDependencyException : DependencyException
{
    public UserDependencyException(Exception innerException)
        : base("User service dependency error occurred", innerException) {
    }

    public UserDependencyException(string message, Exception innerException)
        : base(message, innerException) {
    }
}