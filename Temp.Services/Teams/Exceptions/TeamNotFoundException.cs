using System;

namespace Temp.Services.Teams.Exceptions
{
    public class TeamNotFoundException : Exception
    {
        public TeamNotFoundException() : base("The specified team was not found.")
        {
        }

        public TeamNotFoundException(string message) : base(message)
        {
        }

        public TeamNotFoundException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}