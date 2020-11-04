﻿using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Temp.Domain.Models.EmploymentStatuses.Exceptions;

namespace Temp.Application.EmploymentStatuses.Service
{
    public partial class EmploymentStatusService
    {
        public delegate Task<CreateEmploymentStatus.Response> ReturningEmploymentStatusFunction();
        public delegate IEnumerable<GetEmploymentStatuses.EmploymentStatusViewModel> ReturningEmploymentStatusesFunction();
        


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
