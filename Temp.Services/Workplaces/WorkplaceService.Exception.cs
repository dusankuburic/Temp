using Microsoft.Data.SqlClient;
using Temp.Services._Helpers;
using Temp.Services.Workplaces.Exceptions;
using Temp.Services.Workplaces.Models.Commands;
using Temp.Services.Workplaces.Models.Queries;

namespace Temp.Services.Workplaces;

public partial class WorkplaceService
{
    public delegate Task<CreateWorkplaceResponse> ReturningWorkplaceResponse();
    public delegate Task<PagedList<GetWorkplacesResponse>> ReturningGetPagedWorkplacesFunction();
    public delegate Task<List<GetWorkplaceResponse>> ReturningGetWorkplacesFunction();
    public delegate Task<GetWorkplaceResponse> ReturningGetWorkplaceFunction();
    public delegate Task<UpdateWorkplaceResponse> ReturningUpdateWorkplaceFunction();
    public delegate Task<UpdateWorkplaceStatusResponse> ReturningUpdateWorkplaceStatusFunction();

    public async Task<CreateWorkplaceResponse> TryCatch(ReturningWorkplaceResponse returningWorkplaceResponse) {
        try {
            return await returningWorkplaceResponse();
        } catch (NullWorkplaceException nullWorkplaceException) {
            throw CreateAndLogValidationException(nullWorkplaceException);
        } catch (InvalidWorkplaceException invalidWorkplaceException) {
            throw CreateAndLogValidationException(invalidWorkplaceException);
        } catch (SqlException sqlException) {
            throw CreateAndLogCriticalDependencyException(sqlException);
        } catch (Exception exception) {
            throw CreateAndLogServiceException(exception);
        }
    }

    public async Task<PagedList<GetWorkplacesResponse>> TryCatch(ReturningGetPagedWorkplacesFunction returningGetPagedWorkplacesFunction) {
        try {
            return await returningGetPagedWorkplacesFunction();
        } catch (SqlException sqlException) {
            throw CreateAndLogValidationException(sqlException);
        } catch (Exception exception) {
            throw CreateAndLogServiceException(exception);
        }
    }

    public async Task<List<GetWorkplaceResponse>> TryCatch(ReturningGetWorkplacesFunction returningGetWorkplacesFunction) {
        try {
            return await returningGetWorkplacesFunction();
        } catch (SqlException sqlException) {
            throw CreateAndLogValidationException(sqlException);
        } catch (Exception exception) {
            throw CreateAndLogServiceException(exception);
        }
    }

    public async Task<GetWorkplaceResponse> TryCatch(ReturningGetWorkplaceFunction returningGetWorkplaceFunction) {
        try {
            return await returningGetWorkplaceFunction();
        } catch (SqlException sqlException) {
            throw CreateAndLogValidationException(sqlException);
        } catch (Exception exception) {
            throw CreateAndLogServiceException(exception);
        }
    }

    public async Task<UpdateWorkplaceResponse> TryCatch(ReturningUpdateWorkplaceFunction returningUpdateWorkplaceFunction) {
        try {
            return await returningUpdateWorkplaceFunction();
        } catch (NullWorkplaceException nullWorkplaceException) {
            throw CreateAndLogValidationException(nullWorkplaceException);
        } catch (InvalidWorkplaceException invalidWorkplaceException) {
            throw CreateAndLogValidationException(invalidWorkplaceException);
        } catch (SqlException sqlException) {
            throw CreateAndLogCriticalDependencyException(sqlException);
        } catch (Exception exception) {
            throw CreateAndLogServiceException(exception);
        }
    }

    public async Task<UpdateWorkplaceStatusResponse> TryCatch(ReturningUpdateWorkplaceStatusFunction returningUpdateWorkplaceStatusFunction) {
        try {
            return await returningUpdateWorkplaceStatusFunction();
        } catch (NullWorkplaceException nullWorkplaceException) {
            throw CreateAndLogValidationException(nullWorkplaceException);
        } catch (InvalidWorkplaceException invalidWorkplaceException) {
            throw CreateAndLogValidationException(invalidWorkplaceException);
        } catch (SqlException sqlException) {
            throw CreateAndLogCriticalDependencyException(sqlException);
        } catch (Exception exception) {
            throw CreateAndLogServiceException(exception);
        }
    }

    private WorkplaceValidationException CreateAndLogValidationException(Exception exception) {
        var workplaceValidaitonException = new WorkplaceValidationException(exception);
        _loggingBroker.LogError(workplaceValidaitonException);
        return workplaceValidaitonException;
    }

    private WorkplaceServiceException CreateAndLogServiceException(Exception exception) {
        var workplaceServiceException = new WorkplaceServiceException(exception);
        _loggingBroker.LogError(workplaceServiceException);
        return workplaceServiceException;
    }

    private WorkplaceDependencyException CreateAndLogCriticalDependencyException(Exception exception) {
        var workplaceDependencyException = new WorkplaceDependencyException(exception);
        _loggingBroker.LogCritical(workplaceDependencyException);
        return workplaceDependencyException;
    }
}
