using System;

namespace Temp.Domain.Models.Applications.Exceptions
{
    public class ApplicationWithTeamStorageException : Exception
    {
        public ApplicationWithTeamStorageException() :
            base("No applications found in storage for chosen team") {

        }
    }
}