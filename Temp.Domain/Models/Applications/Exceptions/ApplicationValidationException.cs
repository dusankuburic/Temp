using System;

namespace Temp.Domain.Models.Applications.Exceptions;

public class ApplicationValidationException : Exception
{
    public ApplicationValidationException(Exception innerException)
        : base("Invalid input, contact support.", innerException) {

    }
}
