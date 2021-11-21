using System;

namespace Temp.Domain.Models.Workplaces.Exceptions
{
    public class WorkplaceServiceException : Exception
    {
        public WorkplaceServiceException(Exception innerException)
            : base("Service error, contact support", innerException) {

        }
    }
}