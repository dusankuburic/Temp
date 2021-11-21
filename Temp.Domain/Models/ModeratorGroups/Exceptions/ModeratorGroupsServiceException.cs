using System;

namespace Temp.Domain.Models.ModeratorGroups.Exceptions
{
    public class ModeratorGroupsServiceException : Exception
    {
        public ModeratorGroupsServiceException(Exception innerException)
            : base("Service error, contact support", innerException) {

        }
    }
}