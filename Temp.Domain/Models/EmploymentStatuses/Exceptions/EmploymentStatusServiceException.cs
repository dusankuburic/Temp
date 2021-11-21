using System;

namespace Temp.Domain.Models.EmploymentStatuses.Exceptions
{
    public class EmploymentStatusServiceException : Exception
    {
        public EmploymentStatusServiceException(Exception innerException)
            : base("Service error, contact support", innerException) {

        }
    }
}