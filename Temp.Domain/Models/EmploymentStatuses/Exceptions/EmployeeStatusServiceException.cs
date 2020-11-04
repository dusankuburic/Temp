using System;

namespace Temp.Domain.Models.EmploymentStatuses.Exceptions
{
    public class EmployeeStatusServiceException : Exception
    {
        public EmployeeStatusServiceException(Exception innerException)
            : base("Service error, contact support", innerException)
        {

        }
    }
}
