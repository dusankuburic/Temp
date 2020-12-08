using System;

namespace Temp.Domain.Models.Workplaces.Exceptions
{
    public class WorkplaceEmptyStorageException : Exception
    {
        public WorkplaceEmptyStorageException() : base("No workplaces found in storage")
        {

        }      
    }
}
