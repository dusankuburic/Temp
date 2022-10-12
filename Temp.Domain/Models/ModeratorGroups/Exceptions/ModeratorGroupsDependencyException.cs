namespace Temp.Domain.Models.ModeratorGroups.Exceptions;

public class ModeratorGroupsDependencyException : Exception
{
    public ModeratorGroupsDependencyException(Exception innerException)
        : base("Service dependency error occurred, contact support", innerException) {

    }
}
