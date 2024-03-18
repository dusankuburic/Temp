namespace Temp.Services.Auth.Exceptions;

public class AlreadyExistsUserException : Exception
{
    public AlreadyExistsUserException(Exception innerException)
        : base("User with same id already exists.", innerException) { }
}
