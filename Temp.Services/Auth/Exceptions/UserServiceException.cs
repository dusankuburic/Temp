namespace Temp.Services.Auth.Exceptions;

public class UserServiceException : Exception
{
    public UserServiceException(Exception innerException)
        : base("Service error occurred, contact support.", innerException) { }
}
