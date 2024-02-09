using Microsoft.Data.SqlClient;
using Temp.Core._Helpers;
using Temp.Services.Workplaces.Exceptions;
using Temp.Services.Workplaces.Models.Command;
using Temp.Services.Workplaces.Models.Query;

namespace Temp.Services.Workplaces;

public partial class WorkplaceService
{
    public delegate Task<CreateWorkplaceResponse> ReturningWorkplaceResponse();
    public delegate Task<PagedList<GetWorkplacesResponse>> ReturningGetPagedWorkplacesFunction();
    public delegate Task<IEnumerable<GetWorkplaceResponse>> ReturningGetWorkplacesFunction();
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

    public async Task<IEnumerable<GetWorkplaceResponse>> TryCatch(ReturningGetWorkplacesFunction returningGetWorkplacesFunction) {
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

        return workplaceValidaitonException;
    }

    private WorkplaceServiceException CreateAndLogServiceException(Exception exception) {
        var workplaceServiceException = new WorkplaceServiceException(exception);

        return workplaceServiceException;
    }

    private WorkplaceDependencyException CreateAndLogCriticalDependencyException(Exception exception) {
        var workplaceDependencyException = new WorkplaceDependencyException(exception);

        return workplaceDependencyException;
    }
}
