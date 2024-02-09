namespace Temp.Services.Groups.Exceptions;

public class GroupDependencyException : Exception
{
    public GroupDependencyException(Exception innerException)
        : base("Service dependency error occurred, contact supprot", innerException) {
    }
}
