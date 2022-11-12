using Temp.Domain.Models.EmploymentStatuses.Exceptions;
using Temp.Domain.Models.Engagements.Exceptions;
using Temp.Domain.Models.Workplaces.Exceptions;

namespace Temp.Core.Engagements;

public partial class EngagementService
{
    public delegate Task<CreateEngagement.Response> ReturningCreateEngagementFunction();

    public delegate Task<string> ReturningCreateEngagementViewModelFunction();

    public delegate Task<IEnumerable<GetUserEmployeeEngagements.Response>> ReturningGetUserEmployeeEngagements();

    public async Task<IEnumerable<GetUserEmployeeEngagements.Response>> TryCatch(ReturningGetUserEmployeeEngagements returningGetUserEmployeeEngagements) {
        try {
            return await returningGetUserEmployeeEngagements();
        } catch (NullUserException nullUserException) {
            throw CreateAndLogValidationException(nullUserException);
        } catch (NullEngagementException nullEngagementException) {
            throw CreateAndLogValidationException(nullEngagementException);
        } catch (SqlException sqlException) {
            throw CreateAndLogCriticalDependencyException(sqlException);
        } catch (Exception exception) {
            throw CreateAndLogServiceException(exception);
        }

    }

    public async Task<CreateEngagement.Response> TryCatch(ReturningCreateEngagementFunction returningCreateEngagementFunction) {
        try {
            return await returningCreateEngagementFunction();
        } catch (NullEngagementException nullEngagementException) {
            throw CreateAndLogValidationException(nullEngagementException);
        } catch (InvalidEngagementException invalidEngagementException) {
            throw CreateAndLogValidationException(invalidEngagementException);
        } catch (DateRangeEngagementException dateRangeEngagementException) {
            throw CreateAndLogValidationException(dateRangeEngagementException);
        } catch (SqlException sqlException) {
            throw CreateAndLogCriticalDependencyException(sqlException);
        } catch (Exception exception) {
            throw CreateAndLogServiceException(exception);
        }
    }

    //public async Task<string> TryCatch(ReturningCreateEngagementViewModelFunction returningCreateEngagementViewModelFunction) {
    //    try {
    //        return await returningCreateEngagementViewModelFunction();
    //    } catch (NullEmployeeException nullEmployeeException) {
    //        throw CreateAndLogServiceEmployeeException(nullEmployeeException);
    //    } catch (NullWorkplaceException nullWorkplaceException) {
    //        throw CreateAndLogValidationWorkplaceException(nullWorkplaceException);
    //    } catch (NullEmploymentStatusException nullEmploymentStatusException) {
    //        throw CreateAndLogValidationEmploymentStatusException(nullEmploymentStatusException);
    //    } catch (SqlException sqlException) {
    //        throw CreateAndLogCriticalDependencyException(sqlException);
    //    } catch (Exception exception) {
    //        throw CreateAndLogServiceException(exception);
    //    }

    //}


    private EngagementServiceException CreateAndLogServiceException(Exception exception) {
        var engagementServiceException = new EngagementServiceException(exception);
        //LOG
        return engagementServiceException;
    }

    private EngagementValidationException CreateAndLogValidationException(Exception exception) {
        var engagementValidationException = new EngagementValidationException(exception);
        //LOG
        return engagementValidationException;
    }

    private EngagementDependencyException CreateAndLogCriticalDependencyException(Exception exception) {
        var engagementDependencyException = new EngagementDependencyException(exception);
        //LOG
        return engagementDependencyException;
    }


    //private EmployeeValidationException CreateAndLogServiceEmployeeException(Exception exception) {
    //    var employeeValidationException = new EmployeeValidationException(exception);
    //    //LOG
    //    return employeeValidationException;
    //}

    private WorkplaceValidationException CreateAndLogValidationWorkplaceException(Exception exception) {
        var workplaceValidationException = new WorkplaceValidationException(exception);
        //LOG
        return workplaceValidationException;
    }

    private EmploymentStatusValidationException CreateAndLogValidationEmploymentStatusException(Exception exception) {
        var employmentStatusValidationException = new EmploymentStatusValidationException(exception);

        return employmentStatusValidationException;
    }
}
