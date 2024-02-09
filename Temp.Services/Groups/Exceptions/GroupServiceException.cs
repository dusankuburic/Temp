namespace Temp.Services.Groups.Exceptions;

public class GroupServiceException : Exception
{
    public GroupServiceException(Exception innerException)
        : base("Service Error, contact support", innerException) { }
}
