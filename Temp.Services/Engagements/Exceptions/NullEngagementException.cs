namespace Temp.Services.Engagements.Exceptions;

public class NullEngagementException : Exception
{
    public NullEngagementException() : base("Engagement is null") { }
}
