using Temp.Services.Exceptions;

namespace Temp.Services.Groups.Exceptions;

public class GroupDependencyException : DependencyException
{
    public GroupDependencyException(Exception innerException)
        : base("Group service dependency error occurred", innerException) {
    }

    public GroupDependencyException(string message, Exception innerException)
        : base(message, innerException) {
    }
}