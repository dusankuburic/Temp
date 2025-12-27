using Temp.Services.Exceptions;

namespace Temp.Services.EmploymentStatuses.Exceptions;

public class EmploymentStatusServiceException : ServiceException
{
    public EmploymentStatusServiceException(Exception innerException)
        : base("EmploymentStatus service error occurred", "EMPLOYMENT_STATUS_SERVICE_ERROR", innerException) {
    }

    public EmploymentStatusServiceException(string message, Exception innerException)
        : base(message, "EMPLOYMENT_STATUS_SERVICE_ERROR", innerException) {
    }
}