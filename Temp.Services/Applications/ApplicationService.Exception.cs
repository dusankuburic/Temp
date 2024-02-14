using Microsoft.Data.SqlClient;
using Temp.Services.Applications.Exceptions;
using Temp.Services.Applications.Models.Commands;
using Temp.Services.Applications.Models.Queries;

namespace Temp.Services.Applications
{
    public partial class ApplicationService
    {
        public delegate Task<CreateApplicationResponse> ReturningCreateApplicationFunction();
        public delegate Task<UpdateApplicationStatusResponse> ReturningUpdateApplicationStatusFunction();
        public delegate Task<GetApplicationResponse> ReturningGetApplicationFunction();
        public delegate Task<IEnumerable<GetTeamApplicationsResponse>> ReturningGetTeamApplicationsFunction();
        public delegate Task<IEnumerable<GetUserApplicationsResponse>> ReturningGetUserApplicationsFunction();


        public async Task<CreateApplicationResponse> TryCatch(ReturningCreateApplicationFunction returningCreateApplicationFunction) {
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

        public async Task<UpdateApplicationStatusResponse> TryCatch(ReturningUpdateApplicationStatusFunction returningUpdateApplicationStatusFunction) {
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

        public async Task<GetApplicationResponse> TryCatch(ReturningGetApplicationFunction returningGetApplicationFunction) {
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

        public async Task<IEnumerable<GetTeamApplicationsResponse>> TryCatch(ReturningGetTeamApplicationsFunction returningGetTeamApplicationsFunction) {
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

        public async Task<IEnumerable<GetUserApplicationsResponse>> TryCatch(ReturningGetUserApplicationsFunction returningGetUserApplicationsFunction) {
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
}
