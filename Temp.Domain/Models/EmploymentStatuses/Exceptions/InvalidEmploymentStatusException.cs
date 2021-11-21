using System;

namespace Temp.Domain.Models.EmploymentStatuses.Exceptions
{
    public class InvalidEmploymentStatusException : Exception
    {
        public InvalidEmploymentStatusException(string parameterName, object parameterValue)
            : base($"Invalid Employee, " +
                  $"Parameter Name : {parameterName}, " +
                  $"Parameter Value : {parameterValue}.") { }
    }
}