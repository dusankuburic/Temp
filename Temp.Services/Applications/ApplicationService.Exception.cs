using Temp.Services.Applications.Exceptions;
using Temp.Services.Applications.Models.Commands;
using Temp.Services.Applications.Models.Queries;

namespace Temp.Services.Applications
{
    public partial class ApplicationService
    {
        private delegate Task<CreateApplicationResponse> ReturningCreateApplicationFunction();
        private delegate Task<UpdateApplicationStatusResponse> ReturningUpdateApplicationStatusFunction();
        private delegate Task<GetApplicationResponse> ReturningGetApplicationFunction();
        private delegate Task<IEnumerable<GetTeamApplicationsResponse>> ReturningGetTeamApplicationsFunction();
        private delegate Task<IEnumerable<GetUserApplicationsResponse>> ReturningGetUserApplicationsFunction();

        private async Task<CreateApplicationResponse> TryCatch(ReturningCreateApplicationFunction returningCreateApplicationFunction) {
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

        private async Task<UpdateApplicationStatusResponse> TryCatch(ReturningUpdateApplicationStatusFunction returningUpdateApplicationStatusFunction) {
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

        private async Task<GetApplicationResponse> TryCatch(ReturningGetApplicationFunction returningGetApplicationFunction) {
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

        private async Task<IEnumerable<GetTeamApplicationsResponse>> TryCatch(ReturningGetTeamApplicationsFunction returningGetTeamApplicationsFunction) {
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

        private async Task<IEnumerable<GetUserApplicationsResponse>> TryCatch(ReturningGetUserApplicationsFunction returningGetUserApplicationsFunction) {
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
            Logger.LogError(applicationServiceException);
            return applicationServiceException;
        }

        private ApplicationValidationException CreateAndLogValidationException(Exception exception) {
            var applicationValidationException = new ApplicationValidationException(exception);
            Logger.LogError(applicationValidationException);
            return applicationValidationException;
        }

        private ApplicationDependencyException CreateAndLogCriticalDependencyException(Exception exception) {
            var applicationDependencyException = new ApplicationDependencyException(exception);
            Logger.LogCritical(applicationDependencyException);
            return applicationDependencyException;
        }
    }
}