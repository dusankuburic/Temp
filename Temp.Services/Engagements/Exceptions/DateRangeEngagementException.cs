using ValidationEx = Temp.Services.Exceptions.ValidationException;

namespace Temp.Services.Engagements.Exceptions;

public class DateRangeEngagementException : ValidationEx
{
    public DateRangeEngagementException()
        : base("Invalid date range", "DateRange", "Date From cannot be higher than Date To") {
    }
}