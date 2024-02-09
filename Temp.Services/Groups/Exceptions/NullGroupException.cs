namespace Temp.Services.Groups.Exceptions;

public class NullGroupException : Exception
{
    public NullGroupException() : base("Group is null") {

    }
}
