using Temp.Services._Helpers;
using Temp.Services.Workplaces.Exceptions;
using Temp.Services.Workplaces.Models.Commands;
using Temp.Services.Workplaces.Models.Queries;

namespace Temp.Services.Workplaces;

public partial class WorkplaceService
{
    private delegate Task<CreateWorkplaceResponse> ReturningWorkplaceResponse();
    private delegate Task<PagedList<GetWorkplacesResponse>> ReturningGetPagedWorkplacesFunction();
    private delegate Task<List<GetWorkplaceResponse>> ReturningGetWorkplacesFunction();
    private delegate Task<GetWorkplaceResponse> ReturningGetWorkplaceFunction();
    private delegate Task<UpdateWorkplaceResponse> ReturningUpdateWorkplaceFunction();
    private delegate Task<UpdateWorkplaceStatusResponse> ReturningUpdateWorkplaceStatusFunction();
    private delegate Task<bool> ReturningWorkplaceExistsFunction();

    private async Task<CreateWorkplaceResponse> TryCatch(ReturningWorkplaceResponse returningWorkplaceResponse) {
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

    private async Task<PagedList<GetWorkplacesResponse>> TryCatch(ReturningGetPagedWorkplacesFunction returningGetPagedWorkplacesFunction) {
        try {
            return await returningGetPagedWorkplacesFunction();
        } catch (SqlException sqlException) {
            throw CreateAndLogValidationException(sqlException);
        } catch (Exception exception) {
            throw CreateAndLogServiceException(exception);
        }
    }

    private async Task<List<GetWorkplaceResponse>> TryCatch(ReturningGetWorkplacesFunction returningGetWorkplacesFunction) {
        try {
            return await returningGetWorkplacesFunction();
        } catch (SqlException sqlException) {
            throw CreateAndLogValidationException(sqlException);
        } catch (Exception exception) {
            throw CreateAndLogServiceException(exception);
        }
    }

    private async Task<GetWorkplaceResponse> TryCatch(ReturningGetWorkplaceFunction returningGetWorkplaceFunction) {
        try {
            return await returningGetWorkplaceFunction();
        } catch (SqlException sqlException) {
            throw CreateAndLogValidationException(sqlException);
        } catch (Exception exception) {
            throw CreateAndLogServiceException(exception);
        }
    }

    private async Task<UpdateWorkplaceResponse> TryCatch(ReturningUpdateWorkplaceFunction returningUpdateWorkplaceFunction) {
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

    private async Task<UpdateWorkplaceStatusResponse> TryCatch(ReturningUpdateWorkplaceStatusFunction returningUpdateWorkplaceStatusFunction) {
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

    private async Task<bool> TryCatch(ReturningWorkplaceExistsFunction returningWorkplaceExistsFunction) {
        try {
            return await returningWorkplaceExistsFunction();
        } catch (SqlException sqlException) {
            throw CreateAndLogCriticalDependencyException(sqlException);
        } catch (Exception exception) {
            throw CreateAndLogServiceException(exception);
        }
    }

    private WorkplaceValidationException CreateAndLogValidationException(Exception exception) {
        var workplaceValidaitonException = new WorkplaceValidationException(exception);
        Logger.LogError(workplaceValidaitonException);
        return workplaceValidaitonException;
    }

    private WorkplaceServiceException CreateAndLogServiceException(Exception exception) {
        var workplaceServiceException = new WorkplaceServiceException(exception);
        Logger.LogError(workplaceServiceException);
        return workplaceServiceException;
    }

    private WorkplaceDependencyException CreateAndLogCriticalDependencyException(Exception exception) {
        var workplaceDependencyException = new WorkplaceDependencyException(exception);
        Logger.LogCritical(workplaceDependencyException);
        return workplaceDependencyException;
    }
}