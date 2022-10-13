namespace Temp.Domain.Models.Groups.Exceptions;

public class GroupServiceException : Exception
{
    public GroupServiceException(Exception innerException)
        : base("Service error, contact support", innerException) {

    }
}
