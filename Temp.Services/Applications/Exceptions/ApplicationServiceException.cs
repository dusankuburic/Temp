using Temp.Services.Exceptions;

namespace Temp.Services.Applications.Exceptions;

public class ApplicationServiceException : ServiceException
{
    public ApplicationServiceException(Exception innerException)
        : base("Application service error occurred", "APPLICATION_SERVICE_ERROR", innerException) {
    }

    public ApplicationServiceException(string message, Exception innerException)
        : base(message, "APPLICATION_SERVICE_ERROR", innerException) {
    }
}