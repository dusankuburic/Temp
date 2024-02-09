namespace Temp.Services.Groups.Exceptions;

public class GroupEmptyStorageException : Exception
{
    public GroupEmptyStorageException()
        : base("No Groups found in storage") {

    }
}
