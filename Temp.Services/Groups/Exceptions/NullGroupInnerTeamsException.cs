namespace Temp.Services.Groups.Exceptions;

public class NullGroupInnerTeamsException : Exception
{
    public NullGroupInnerTeamsException()
        : base("Inner Team is null") {

    }
}
