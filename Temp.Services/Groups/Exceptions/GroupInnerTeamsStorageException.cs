using Temp.Services.Exceptions;

namespace Temp.Services.Groups.Exceptions;

public class GroupInnerTeamsStorageException : NotFoundException
{
    public GroupInnerTeamsStorageException()
        : base("There are no Inner Teams inside Group") {
    }
}