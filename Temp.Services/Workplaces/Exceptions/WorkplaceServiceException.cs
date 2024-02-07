namespace Temp.Services.Workplaces.Exceptions;

public class WorkplaceServiceException : Exception
{
    public WorkplaceServiceException(Exception innerException)
        : base("Service error, contact supprot", innerException) { }
}
