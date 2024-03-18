namespace Temp.Services.Auth.Exceptions;

public class UserValidationException : Exception
{
    public UserValidationException(Exception innerException)
        : base("Invalid input, contact support.", innerException) { }
}
