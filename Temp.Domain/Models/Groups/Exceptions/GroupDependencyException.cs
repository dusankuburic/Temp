namespace Temp.Domain.Models.Groups.Exceptions;

public class GroupDependencyException : Exception
{
    public GroupDependencyException(Exception innerException)
        : base("Service dependency error occurred, contact support", innerException) {

    }
}
