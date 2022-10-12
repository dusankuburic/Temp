using Temp.Domain.Models.Applications.Exceptions;

namespace Temp.Core.Applications.Service;

public partial class ApplicationService
{
    public delegate Task<CreateApplication.Response> ReturningCreateApplicationFunction();
    public delegate Task<GetApplication.ApplicationViewModel> ReturningGetApplicationFunction();
    public delegate Task<IEnumerable<GetTeamApplications.ApplicationViewModel>> ReturningGetTeamApplicationsFunction();
    public delegate Task<IEnumerable<GetUserApplications.ApplicationViewModel>> ReturningGetUserApplicationsFunction();
    public delegate Task<UpdateApplicationStatus.Response> ReturningUpdateApplicationStatusFunction();


    public async Task<UpdateApplicationStatus.Response> TryCatch(ReturningUpdateApplicationStatusFunction returningUpdateApplicationStatusFunction) {
        try {
            return await returningUpdateApplicationStatusFunction();
        } catch (NullApplicationException nullApplicationException) {
            throw CreateAndLogValidationException(nullApplicationException);
        } catch (SqlException sqlException) {
            throw CreateAndLogCriticalDependencyException(sqlException);
        } catch (Exception exception) {
            throw CreateAndLogServiceException(exception);
        }
    }


    public async Task<CreateApplication.Response> TryCatch(ReturningCreateApplicationFunction returningCreateApplicationFunction) {
        try {
            return await returningCreateApplicationFunction();
        } catch (NullApplicationException nullApplicationException) {
            throw CreateAndLogValidationException(nullApplicationException);
        } catch (InvalidApplicationException invalidApplicationException) {
            throw CreateAndLogValidationException(invalidApplicationException);
        } catch (SqlException sqlException) {
            throw CreateAndLogCriticalDependencyException(sqlException);
        } catch (Exception exception) {
            throw CreateAndLogServiceException(exception);
        }
    }

    public async Task<GetApplication.ApplicationViewModel> TryCatch(ReturningGetApplicationFunction returningGetApplicationFunction) {
        try {
            return await returningGetApplicationFunction();
        } catch (NullApplicationException nullApplicationException) {
            throw CreateAndLogValidationException(nullApplicationException);
        } catch (SqlException sqlException) {
            throw CreateAndLogCriticalDependencyException(sqlException);
        } catch (Exception exception) {
            throw CreateAndLogServiceException(exception);
        }
    }

    public async Task<IEnumerable<GetTeamApplications.ApplicationViewModel>> TryCatch(ReturningGetTeamApplicationsFunction returningGetTeamApplicationsFunction) {
        try {
            return await returningGetTeamApplicationsFunction();
        } catch (ApplicationWithTeamStorageException applicationWithTeamStorageException) {
            throw CreateAndLogValidationException(applicationWithTeamStorageException);
        } catch (SqlException sqlException) {
            throw CreateAndLogCriticalDependencyException(sqlException);
        } catch (Exception exception) {
            throw CreateAndLogServiceException(exception);
        }
    }

    public async Task<IEnumerable<GetUserApplications.ApplicationViewModel>> TryCatch(ReturningGetUserApplicationsFunction returningGetUserApplicationsFunction) {
        try {
            return await returningGetUserApplicationsFunction();
        } catch (ApplicationWithUserStorageException applicationWithUserStorageException) {
            throw CreateAndLogValidationException(applicationWithUserStorageException);
        } catch (SqlException sqlException) {
            throw CreateAndLogCriticalDependencyException(sqlException);
        } catch (Exception exception) {
            throw CreateAndLogServiceException(exception);
        }
    }

    private ApplicationServiceException CreateAndLogServiceException(Exception exception) {
        var applicationServiceException = new ApplicationServiceException(exception);
        //LOG
        return applicationServiceException;
    }

    private ApplicationValidationException CreateAndLogValidationException(Exception exception) {
        var applicationValidationException = new ApplicationValidationException(exception);
        //LOG
        return applicationValidationException;
    }

    private ApplicationDependencyException CreateAndLogCriticalDependencyException(Exception exception) {
        var applicationDependencyException = new ApplicationDependencyException(exception);
        //LOG
        return applicationDependencyException;
    }


}
