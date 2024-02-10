namespace Temp.Services.Engagements.Exceptions;

public class DateRangeEngagementException : Exception
{
    public DateRangeEngagementException()
        : base("Date From cant be higher than Date To") { }
}
