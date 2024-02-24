using Microsoft.Data.SqlClient;
using Temp.Services._Helpers;
using Temp.Services.EmploymentStatuses.Exceptions;
using Temp.Services.EmploymentStatuses.Models.Commands;
using Temp.Services.EmploymentStatuses.Models.Queries;

namespace Temp.Services.EmploymentStatuses;

public partial class EmploymentStatusService
{
    public delegate Task<CreateEmploymentStatusResponse> ReturningEmploymentStatusFunction();
    public delegate Task<List<GetEmploymentStatusResponse>> ReturningEmploymentStatusesFunction();
    public delegate Task<PagedList<GetPagedEmploymentStatusesResponse>> ReturningPagedEmplymentStatusesFunction();
    public delegate Task<GetEmploymentStatusResponse> ReturningGetEmploymentStatusFunction();
    public delegate Task<UpdateEmploymentStatusResponse> ReturningUpdateEmploymentStatusFunction();
    public delegate Task<UpdateEmploymentStatusStatusResponse> ReturningUpdateEmploymentStatusStatusFunction();

    public async Task<CreateEmploymentStatusResponse> TryCatch(ReturningEmploymentStatusFunction returningEmploymentStatusFunction) {
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

    public async Task<List<GetEmploymentStatusResponse>> TryCatch(ReturningEmploymentStatusesFunction returningEmploymentStatusesFunction) {
        try {
            return await returningEmploymentStatusesFunction();
        } catch (SqlException sqlException) {
            throw CreateAndLogCriticalDependencyException(sqlException);
        } catch (Exception exception) {
            throw CreateAndLogServiceException(exception);
        }
    }

    public async Task<PagedList<GetPagedEmploymentStatusesResponse>> TryCatch(ReturningPagedEmplymentStatusesFunction returningPagedEmplymentStatusesFunction) {
        try {
            return await returningPagedEmplymentStatusesFunction();
        } catch (SqlException sqlException) {
            throw CreateAndLogCriticalDependencyException(sqlException);
        } catch (Exception exception) {
            throw CreateAndLogServiceException(exception);
        }
    }

    public async Task<GetEmploymentStatusResponse> TryCatch(ReturningGetEmploymentStatusFunction returningGetEmploymentStatusFunction) {
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

    public async Task<UpdateEmploymentStatusResponse> TryCatch(ReturningUpdateEmploymentStatusFunction returningUpdateEmploymentStatusFunction) {
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

    public async Task<UpdateEmploymentStatusStatusResponse> TryCatch(ReturningUpdateEmploymentStatusStatusFunction returningUpdateEmploymentStatusStatusFunction) {
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

    private EmploymentStatusServiceException CreateAndLogServiceException(Exception exception) {
        var employmentStatusServiceException = new EmploymentStatusServiceException(exception);
        _loggingBroker.LogError(employmentStatusServiceException);
        return employmentStatusServiceException;
    }

    private EmploymentStatusValidationException CreateAndLogValidationException(Exception exception) {
        var employmentStatusValidationException = new EmploymentStatusValidationException(exception);
        _loggingBroker.LogError(employmentStatusValidationException);
        return employmentStatusValidationException;
    }

    private EmploymentStatusDependencyException CreateAndLogCriticalDependencyException(Exception exception) {
        var employmentStatusDependencyException = new EmploymentStatusDependencyException(exception);
        _loggingBroker.LogCritical(employmentStatusDependencyException);
        return employmentStatusDependencyException;
    }
}
