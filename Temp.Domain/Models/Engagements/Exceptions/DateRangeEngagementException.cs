namespace Temp.Domain.Models.Engagements.Exceptions;

public class DateRangeEngagementException : Exception
{
    public DateRangeEngagementException() : base("Date from cant be higher than Date to") { }
}
