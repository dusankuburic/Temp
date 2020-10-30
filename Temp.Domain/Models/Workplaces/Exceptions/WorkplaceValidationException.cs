using System;

namespace Temp.Domain.Models.Workplaces.Exceptions
{
    public class WorkplaceValidationException : Exception
    {
        public WorkplaceValidationException(Exception innerException)
            : base("Invalid input, contact support", innerException)
        {

        }
    }
}
