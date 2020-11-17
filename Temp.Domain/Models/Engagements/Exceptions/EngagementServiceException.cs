using System;

namespace Temp.Domain.Models.Engagements.Exceptions
{
    public class EngagementServiceException : Exception
    {
        public EngagementServiceException(Exception innerException)
            : base("Service error, contact support", innerException)
        {
                
        }
    }
}
