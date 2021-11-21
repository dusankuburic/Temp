using System;

namespace Temp.Domain.Models.EmploymentStatuses.Exceptions
{
    public class EmploymentStatusEmptyStorageException : Exception
    {
        public EmploymentStatusEmptyStorageException() : base("No EmploymentStatuses found in storage") {

        }
    }
}