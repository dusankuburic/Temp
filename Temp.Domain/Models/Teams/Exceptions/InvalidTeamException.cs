using System;

namespace Temp.Domain.Models.Teams.Exceptions
{
    public class InvalidTeamException : Exception
    {
        public InvalidTeamException(string parameterName, object parameterValue)
            : base($"Invalid Team, " +
                  $"Parameter Name: {parameterName}, " +
                  $"Parameter Value: {parameterValue}.") {

        }
    }
}