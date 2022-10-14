namespace Temp.Services.Teams.Exceptions;

public class TeamEmptyStorageException : Exception
{
    public TeamEmptyStorageException() : base("No teams found in storage") {

    }
}

