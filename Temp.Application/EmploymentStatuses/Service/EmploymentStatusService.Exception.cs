using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Temp.Application.Helpers;
using Temp.Domain.Models.EmploymentStatuses.Exceptions;

namespace Temp.Application.EmploymentStatuses.Service
{
    public partial class EmploymentStatusService
    {
        public delegate Task<CreateEmploymentStatus.Response> ReturningEmploymentStatusFunction();
        public delegate IEnumerable<GetEmploymentStatuses.EmploymentStatusViewModel> ReturningEmploymentStatusesFunction();
        public delegate Task<PagedList<GetEmploymentStatuses.EmploymentStatusViewModel>> ReturningEmploymentStatusesFunctionPage();
        public delegate GetEmploymentStatus.EmploymentStatusViewModel ReturningGetEmploymentStatusFunction();
        public delegate Task<UpdateEmploymentStatus.Response> ReturningUpdateEmploymentStatusFunction();

        public async Task<CreateEmploymentStatus.Response> TryCatch(ReturningEmploymentStatusFunction returningEmploymentStatusFunction)
        {
            try
            {
                return await returningEmploymentStatusFunction();
            }
            catch(NullEmploymentStatusException nullEmploymentStatusException)
            {
                throw CreateAndLogValidationException(nullEmploymentStatusException);
            }
            catch(InvalidEmploymentStatusException invalidEmploymentStatusException)
            {
                throw CreateAndLogValidationException(invalidEmploymentStatusException);
            }
            catch(SqlException sqlException)
            {
                throw CreateAndLogCriticalDependencyException(sqlException);
            }
            catch(Exception exception)
            {
                throw CreateAndLogServiceException(exception);
            }
        }
        
        public IEnumerable<GetEmploymentStatuses.EmploymentStatusViewModel> TryCatch(ReturningEmploymentStatusesFunction returningEmploymentStatusesFunction)
        {
            try
            {
                return returningEmploymentStatusesFunction();
            }
            catch(EmploymentStatusEmptyStorageException employmentStatusEmptyStorageException)
            {
                throw CreateAndLogValidationException(employmentStatusEmptyStorageException);
            }
            catch(SqlException sqlException)
            {
                throw CreateAndLogCriticalDependencyException(sqlException);
            }
            catch(Exception exception)
            {
                throw CreateAndLogServiceException(exception);
            }

        }

        public async Task<PagedList<GetEmploymentStatuses.EmploymentStatusViewModel>> TryCatch(ReturningEmploymentStatusesFunctionPage returningEmploymentStatusesFunctionPage)
        {
            try
            {
                return await returningEmploymentStatusesFunctionPage();
            }
            catch(EmploymentStatusEmptyStorageException employmentStatusEmptyStorageException)
            {
                throw CreateAndLogValidationException(employmentStatusEmptyStorageException);
            }
            catch(SqlException sqlException)
            {
                throw CreateAndLogCriticalDependencyException(sqlException);
            }
            catch(Exception exception)
            {
                throw CreateAndLogServiceException(exception);
            }

        }

        public  GetEmploymentStatus.EmploymentStatusViewModel TryCatch(ReturningGetEmploymentStatusFunction returningGetEmploymentStatusFunction)
        {
            try
            {
                return returningGetEmploymentStatusFunction();
            }
            catch (NullEmploymentStatusException nullEmploymentStatusException)
            {
                throw CreateAndLogValidationException(nullEmploymentStatusException);
            }
            catch (SqlException sqlException)
            {
                throw CreateAndLogCriticalDependencyException(sqlException);
            }
            catch(Exception exception)
            {
                throw CreateAndLogServiceException(exception);
            }
        }

        public async Task<UpdateEmploymentStatus.Response> TryCatch(ReturningUpdateEmploymentStatusFunction returningUpdateEmploymentStatusFunction)
        {
            try
            {
                return await returningUpdateEmploymentStatusFunction();
            }
            catch(NullEmploymentStatusException nullEmploymentStatusException)
            {
                throw CreateAndLogValidationException(nullEmploymentStatusException);
            }
            catch(InvalidEmploymentStatusException invalidEmploymentStatusException)
            {
                throw CreateAndLogValidationException(invalidEmploymentStatusException);
            }
            catch (SqlException sqlException)
            {
                throw CreateAndLogCriticalDependencyException(sqlException);
            }
            catch(Exception exception)
            {
                throw CreateAndLogServiceException(exception);
            }
        }

        private EmploymentStatusServiceException CreateAndLogServiceException(Exception exception)
        {
            var employmentStatusServiceException = new EmploymentStatusServiceException(exception);
            //LOG
            return employmentStatusServiceException;
        }

        private EmploymentStatusValidationException CreateAndLogValidationException(Exception exception)
        {
            var employmentStatusValidationException = new EmploymentStatusValidationException(exception);

            return employmentStatusValidationException;
        }

        private EmploymentStatusDependencyException CreateAndLogCriticalDependencyException(Exception exception)
        {
            var employmentStatusDependencyException = new EmploymentStatusDependencyException(exception);
            //LOG
            return employmentStatusDependencyException;
        }
    }
}
