using Temp.Services._Helpers;
using Temp.Services.EmploymentStatuses.Exceptions;
using Temp.Services.EmploymentStatuses.Models.Commands;
using Temp.Services.EmploymentStatuses.Models.Queries;

namespace Temp.Services.EmploymentStatuses;

public partial class EmploymentStatusService
{
    private delegate Task<CreateEmploymentStatusResponse> ReturningEmploymentStatusFunction();
    private delegate Task<List<GetEmploymentStatusResponse>> ReturningEmploymentStatusesFunction();
    private delegate Task<PagedList<GetPagedEmploymentStatusesResponse>> ReturningPagedEmplymentStatusesFunction();
    private delegate Task<GetEmploymentStatusResponse> ReturningGetEmploymentStatusFunction();
    private delegate Task<UpdateEmploymentStatusResponse> ReturningUpdateEmploymentStatusFunction();
    private delegate Task<UpdateEmploymentStatusStatusResponse> ReturningUpdateEmploymentStatusStatusFunction();
    private delegate Task<bool> ReturningEmploymentStatusExistsFunction();

    private async Task<CreateEmploymentStatusResponse> TryCatch(ReturningEmploymentStatusFunction returningEmploymentStatusFunction) {
        try {
            return await returningEmploymentStatusFunction();
        } catch (NullEmploymentStatusException nullEmploymentStatusException) {
            throw CreateAndLogValidationException(nullEmploymentStatusException);
        } catch (InvalidEmploymentStatusException invalidEmploymentStatusException) {
            throw CreateAndLogValidationException(invalidEmploymentStatusException);
        } catch (SqlException sqlException) {
            throw CreateAndLogCriticalDependencyException(sqlException);
        } catch (Exception exception) {
            throw CreateAndLogServiceException(exception);
        }
    }

    private async Task<List<GetEmploymentStatusResponse>> TryCatch(ReturningEmploymentStatusesFunction returningEmploymentStatusesFunction) {
        try {
            return await returningEmploymentStatusesFunction();
        } catch (SqlException sqlException) {
            throw CreateAndLogCriticalDependencyException(sqlException);
        } catch (Exception exception) {
            throw CreateAndLogServiceException(exception);
        }
    }

    private async Task<PagedList<GetPagedEmploymentStatusesResponse>> TryCatch(ReturningPagedEmplymentStatusesFunction returningPagedEmplymentStatusesFunction) {
        try {
            return await returningPagedEmplymentStatusesFunction();
        } catch (SqlException sqlException) {
            throw CreateAndLogCriticalDependencyException(sqlException);
        } catch (Exception exception) {
            throw CreateAndLogServiceException(exception);
        }
    }

    private async Task<GetEmploymentStatusResponse> TryCatch(ReturningGetEmploymentStatusFunction returningGetEmploymentStatusFunction) {
        try {
            return await returningGetEmploymentStatusFunction();
        } catch (NullEmploymentStatusException nullEmploymentStatusException) {
            throw CreateAndLogValidationException(nullEmploymentStatusException);
        } catch (SqlException sqlException) {
            throw CreateAndLogCriticalDependencyException(sqlException);
        } catch (Exception exception) {
            throw CreateAndLogServiceException(exception);
        }
    }

    private async Task<UpdateEmploymentStatusResponse> TryCatch(ReturningUpdateEmploymentStatusFunction returningUpdateEmploymentStatusFunction) {
        try {
            return await returningUpdateEmploymentStatusFunction();
        } catch (NullEmploymentStatusException nullEmploymentStatusException) {
            throw CreateAndLogValidationException(nullEmploymentStatusException);
        } catch (InvalidEmploymentStatusException invalidEmploymentStatusException) {
            throw CreateAndLogValidationException(invalidEmploymentStatusException);
        } catch (SqlException sqlException) {
            throw CreateAndLogCriticalDependencyException(sqlException);
        } catch (Exception exception) {
            throw CreateAndLogServiceException(exception);
        }
    }

    private async Task<UpdateEmploymentStatusStatusResponse> TryCatch(ReturningUpdateEmploymentStatusStatusFunction returningUpdateEmploymentStatusStatusFunction) {
        try {
            return await returningUpdateEmploymentStatusStatusFunction();
        } catch (NullEmploymentStatusException nullEmploymentStatusException) {
            throw CreateAndLogValidationException(nullEmploymentStatusException);
        } catch (InvalidEmploymentStatusException invalidEmploymentStatusException) {
            throw CreateAndLogValidationException(invalidEmploymentStatusException);
        } catch (SqlException sqlException) {
            throw CreateAndLogCriticalDependencyException(sqlException);
        } catch (Exception exception) {
            throw CreateAndLogServiceException(exception);
        }
    }

    private async Task<bool> TryCatch(ReturningEmploymentStatusExistsFunction returningEmploymentStatusExistsFunction) {
        try {
            return await returningEmploymentStatusExistsFunction();
        } catch (SqlException sqlException) {
            throw CreateAndLogCriticalDependencyException(sqlException);
        } catch (Exception exception) {
            throw CreateAndLogServiceException(exception);
        }
    }

    private EmploymentStatusServiceException CreateAndLogServiceException(Exception exception) {
        var employmentStatusServiceException = new EmploymentStatusServiceException(exception);
        Logger.LogError(employmentStatusServiceException);
        return employmentStatusServiceException;
    }

    private EmploymentStatusValidationException CreateAndLogValidationException(Exception exception) {
        var employmentStatusValidationException = new EmploymentStatusValidationException(exception);
        Logger.LogError(employmentStatusValidationException);
        return employmentStatusValidationException;
    }

    private EmploymentStatusDependencyException CreateAndLogCriticalDependencyException(Exception exception) {
        var employmentStatusDependencyException = new EmploymentStatusDependencyException(exception);
        Logger.LogCritical(employmentStatusDependencyException);
        return employmentStatusDependencyException;
    }
}