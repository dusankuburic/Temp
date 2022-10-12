using System;

namespace Temp.Domain.Models.EmploymentStatuses.Exceptions;

public class NullEmploymentStatusException : Exception
{
    public NullEmploymentStatusException() : base("Employment Status in null") { }
}
