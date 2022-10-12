namespace Temp.Domain.Models.Teams.Exceptions;

public class TeamEmptyStorageException : Exception
{
    public TeamEmptyStorageException() : base("No teams found in storage") {

    }
}
