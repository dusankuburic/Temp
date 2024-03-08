using Temp.Domain.Models;
using Temp.Services.Engagements.Exceptions;

namespace Temp.Services.Engagements;

public partial class EngagementService
{
    private void ValidateEngagementOnCreate(Engagement engagement) {
        ValidateEngagement(engagement);
        ValidateEngagementInts(engagement);
        ValidateEngagementDates(engagement);
        ValidateDateRange(engagement);
    }

    private void ValidateEngagement(Engagement engagement) {
        if (engagement is null) {
            throw new NullEngagementException();
        }
    }

    private void ValidateEngagementInts(Engagement engagement) {
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

    private void ValidateEngagementDates(Engagement engagement) {
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

    private void ValidateDateRange(Engagement engagement) {
        if (engagement.DateFrom > engagement.DateTo) {
            throw new DateRangeEngagementException();
        }
    }

    private static bool IsInvalidInt(int input) {
        return input <= 0 || input > int.MaxValue;
    }

    private static bool IsInvalidDate(DateTime input) {
        return input == DateTime.MinValue;
    }
}
