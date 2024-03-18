namespace Temp.Services.Auth.Exceptions;

public class UserDependencyException : Exception
{
    public UserDependencyException(Exception innerException)
        : base("Service dependency error occurred, contact support.", innerException) { }
}
