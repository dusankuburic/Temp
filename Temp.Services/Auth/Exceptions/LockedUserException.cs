namespace Temp.Services.Auth.Exceptions;

public class LockedUserException : Exception
{
    public LockedUserException(Exception innerException)
        : base("Locked user exception, please try again later.", innerException) { }
}
