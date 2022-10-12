using System;

namespace Temp.Domain.Models.Groups.Exceptions;

public class NullGroupException : Exception
{
    public NullGroupException() : base("group is null") {

    }
}
