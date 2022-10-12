using System;

namespace Temp.Domain.Models.Groups.Exceptions;

public class InvalidGroupException : Exception
{
    public InvalidGroupException(string parameterName, object parameterValue)
        : base($"Invalid Group, " +
              $"Parameter Name: {parameterName}, " +
              $"Parameter Value: {parameterValue}.") {

    }
}
