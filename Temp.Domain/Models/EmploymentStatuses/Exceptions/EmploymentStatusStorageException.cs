using System;

namespace Temp.Domain.Models.EmploymentStatuses.Exceptions
{
    public class EmploymentStatusStorageException : Exception
    {
        public EmploymentStatusStorageException() : base("No EmploymentStatuses found in storage")
        {

        }
    }
}
