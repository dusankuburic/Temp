using Temp.Domain.Models;
using Temp.Services.Engagements.Exceptions;

namespace Temp.Services.Engagements;

public partial class EngagementService
{
    public void ValidateEngagementOnCreate(Engagement engagement) {
        ValidateEngagement(engagement);
        ValidateEngagementInts(engagement);
        ValidateEngagementDates(engagement);
        ValidateDateRange(engagement);
    }

    public void ValidateEngagement(Engagement engagement) {
        if (engagement is null) {
            throw new NullEngagementException();
        }
    }

    public void ValidateEngagementInts(Engagement engagement) {
        switch (engagement) {
            case { } when IsInvalidInt(engagement.EmployeeId):
                throw new InvalidEngagementException(
                    parameterName: nameof(engagement.EmployeeId),
                    parameterValue: engagement.EmployeeId);
            case { } when IsInvalidInt(engagement.WorkplaceId):
                throw new InvalidEngagementException(
                    parameterName: nameof(engagement.WorkplaceId),
                    parameterValue: engagement.WorkplaceId);
            case { } when IsInvalidInt(engagement.EmploymentStatusId):
                throw new InvalidEngagementException(
                    parameterName: nameof(engagement.EmploymentStatusId),
                    parameterValue: engagement.EmploymentStatusId);
        }
    }

    public void ValidateEngagementDates(Engagement engagement) {
        switch (engagement) {
            case { } when IsInvalidDate(engagement.DateFrom):
                throw new InvalidEngagementException(
                    parameterName: nameof(engagement.DateFrom),
                    parameterValue: engagement.DateFrom);
            case { } when IsInvalidDate(engagement.DateTo):
                throw new InvalidEngagementException(
                    parameterName: nameof(engagement.DateTo),
                    parameterValue: engagement.DateTo);
        }
    }

    public void ValidateDateRange(Engagement engagement) {
        if (engagement.DateFrom > engagement.DateTo) {
            throw new DateRangeEngagementException();
        }
    }

    public static bool IsInvalidInt(int input) {
        return input <= 0 || input > int.MaxValue;
    }

    public static bool IsInvalidDate(DateTime input) {
        return input == DateTime.MinValue;
    }
}
