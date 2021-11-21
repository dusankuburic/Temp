using System;

namespace Temp.Domain.Models.Teams.Exceptions
{
    public class TeamDependencyException : Exception
    {
        public TeamDependencyException(Exception innerException)
            : base("Service dependency error occurred, contact support", innerException) {

        }

    }
}