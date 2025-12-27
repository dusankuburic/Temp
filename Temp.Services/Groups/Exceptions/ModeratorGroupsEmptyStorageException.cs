using Temp.Services.Exceptions;

namespace Temp.Services.Groups.Exceptions;

public class ModeratorGroupsEmptyStorageException : NotFoundException
{
    public ModeratorGroupsEmptyStorageException()
        : base("Moderator doesn't have any assigned Groups") {
    }
}