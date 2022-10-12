using System;

namespace Temp.Domain.Models.Workplaces.Exceptions;

public class InvalidWorkplaceException : Exception
{
    public InvalidWorkplaceException(string parameterName, object parameterValue)
        : base($"Invalid Employee, " +
              $"Parameter Name : {parameterName}, " +
              $"Parameter Value : {parameterValue}.") { }
}
