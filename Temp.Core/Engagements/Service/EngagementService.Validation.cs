using Temp.Domain.Models;
using Temp.Domain.Models.Employees.Exceptions;
using Temp.Domain.Models.EmploymentStatuses.Exceptions;
using Temp.Domain.Models.Engagements.Exceptions;
using Temp.Domain.Models.Workplaces.Exceptions;

namespace Temp.Core.Engagements;

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

    public void ValidateUser(User user) {
        if (user is null) {
            throw new NullUserException();
        }
    }

    public void ValidateUserEmployeeEngagements(IEnumerable<GetUserEmployeeEngagements.Response> response) {
        if (response is null) {
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

    public void ValidateCreateEngagementViewModel(GetCreateEngagementViewModel.Response response) {
        ValidateEmployee(response);
        ValidateWorkplace(response);
        ValidateEmploymentStatuses(response);
    }


    public void ValidateEmployee(GetCreateEngagementViewModel.Response response) {
        if (response.Employee is null) {
            throw new NullEmployeeException();
        }
    }


    public void ValidateWorkplace(GetCreateEngagementViewModel.Response response) {
        if (response.Workplaces is null) {
            throw new NullWorkplaceException();
        }
    }

    public void ValidateEmploymentStatuses(GetCreateEngagementViewModel.Response response) {
        if (response.EmploymentStatuses is null) {
            throw new NullEmploymentStatusException();
        }
    }


    public static bool IsInvalidInt(int input) {
        if (input > 0 && input <= int.MaxValue) {
            return false;
        }
        return true;
    }

    public static bool IsInvalidDate(DateTime input) {
        if (input != DateTime.MinValue) {
            return false;
        }
        return true;
    }
}
