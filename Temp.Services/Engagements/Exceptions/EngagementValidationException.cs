namespace Temp.Services.Engagements.Exceptions;

public class EngagementValidationException : Exception
{
    public EngagementValidationException(Exception innerException)
        : base("Invalid input, contact support.", innerException) { }
}
