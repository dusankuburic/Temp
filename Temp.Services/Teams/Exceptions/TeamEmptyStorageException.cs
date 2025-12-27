using Temp.Services.Exceptions;

namespace Temp.Services.Teams.Exceptions;

public class TeamEmptyStorageException : NotFoundException
{
    public TeamEmptyStorageException()
        : base("No teams found in storage") {
    }
}