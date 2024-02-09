namespace Temp.Services.Groups.Exceptions;

public class ModeratorGroupsEmptyStorageException : Exception
{
    public ModeratorGroupsEmptyStorageException()
        : base("Moderator doesn't have any assigned Groups") { }
}
