namespace Temp.Services.Auth.Exceptions;

public class NullUserException : Exception
{
    public NullUserException()
        : base("App user is null.") { }
}
