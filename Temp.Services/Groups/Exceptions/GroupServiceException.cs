using Temp.Services.Exceptions;

namespace Temp.Services.Groups.Exceptions;

public class GroupServiceException : ServiceException
{
    public GroupServiceException(Exception innerException)
        : base("Group service error occurred", "GROUP_SERVICE_ERROR", innerException) {
    }

    public GroupServiceException(string message, Exception innerException)
        : base(message, "GROUP_SERVICE_ERROR", innerException) {
    }
}