using System;

namespace Temp.Domain.Models.Workplaces.Exceptions;

public class NullWorkplaceException : Exception
{
    public NullWorkplaceException() : base("workplace is null") { }
}
