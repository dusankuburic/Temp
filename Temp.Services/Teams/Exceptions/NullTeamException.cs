namespace Temp.Services.Teams.Exceptions;

public class NullTeamException : Exception
{
    public NullTeamException() : base("team is null") {

    }
}

