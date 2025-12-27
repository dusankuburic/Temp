namespace Temp.Domain.Models.ModeratorGroups.Exceptions;

public class NullModeratorGroupsException : Exception
{
    public NullModeratorGroupsException() : base("moderatorGroups is null") { }
}