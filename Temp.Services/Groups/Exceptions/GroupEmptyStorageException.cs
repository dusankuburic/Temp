using Temp.Services.Exceptions;

namespace Temp.Services.Groups.Exceptions;

public class GroupEmptyStorageException : NotFoundException
{
    public GroupEmptyStorageException()
        : base("No Groups found in storage") {
    }
}