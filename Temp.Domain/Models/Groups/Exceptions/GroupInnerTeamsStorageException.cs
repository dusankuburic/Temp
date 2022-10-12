using System;

namespace Temp.Domain.Models.Groups.Exceptions;

public class GroupInnerTeamsStorageException : Exception
{
    public GroupInnerTeamsStorageException() :
        base("There are no InnerTeams inside group") {

    }
}
