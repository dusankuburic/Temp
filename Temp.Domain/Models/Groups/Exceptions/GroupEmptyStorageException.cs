namespace Temp.Domain.Models.Groups.Exceptions;

public class GroupEmptyStorageException : Exception
{
    public GroupEmptyStorageException() : base("No groups found in storage") {

    }
}
