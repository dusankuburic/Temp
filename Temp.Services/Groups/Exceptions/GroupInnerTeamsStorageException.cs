namespace Temp.Services.Groups.Exceptions;

public class GroupInnerTeamsStorageException : Exception
{
    public GroupInnerTeamsStorageException()
        : base("There are no Inner Teams inside Group") {

    }
}
