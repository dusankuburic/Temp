using Temp.Services.Exceptions;

namespace Temp.Services.Workplaces.Exceptions;

public class WorkplaceServiceException : ServiceException
{
    public WorkplaceServiceException(Exception innerException)
        : base("Workplace service error occurred", "WORKPLACE_SERVICE_ERROR", innerException) {
    }

    public WorkplaceServiceException(string message, Exception innerException)
        : base(message, "WORKPLACE_SERVICE_ERROR", innerException) {
    }
}