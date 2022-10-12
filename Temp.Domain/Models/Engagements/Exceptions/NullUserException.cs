namespace Temp.Domain.Models.Engagements.Exceptions;

public class NullUserException : Exception
{
    public NullUserException() : base("The user is null") { }
}
