﻿namespace Temp.Services.Groups.Exceptions;

public class GroupValidationException : Exception
{
    public GroupValidationException(Exception innerException)
        : base("Invalid input, contact support", innerException) { }
}
