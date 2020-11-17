using System;

namespace Temp.Domain.Models.Engagements.Exceptions
{
    public class NullEngagementException : Exception
    {
        public NullEngagementException(): base("The engagement is null"){}
    }
}
