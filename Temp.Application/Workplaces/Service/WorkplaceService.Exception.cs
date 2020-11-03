﻿using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Temp.Domain.Models.Workplaces.Exceptions;

namespace Temp.Application.Workplaces.Service
{
    public partial class WorkplaceService
    {
        public delegate Task<CreateWorkplace.Response> ReturningWorkplaceFunction();
        public delegate IEnumerable<GetWorkplaces.WorkplacesViewModel> ReturningGetWorkplacesFunction();

        public async Task<CreateWorkplace.Response> TryCatch(ReturningWorkplaceFunction returningWorkplaceFunction)
        {
            try
            {
                return await returningWorkplaceFunction();
            }
            catch(NullWorkplaceException nullWorkplaceException)
            {
                throw CreateAndLogValidationException(nullWorkplaceException);
            }
            catch(InvalidWorkplaceException invalidWorkplaceException)
            {
                throw CreateAndLogValidationException(invalidWorkplaceException);
            }
        }


        public IEnumerable<GetWorkplaces.WorkplacesViewModel> TryCatch(ReturningGetWorkplacesFunction returningGetWorkplacesFunction)
        {
            try
            {
                return returningGetWorkplacesFunction();
            }
            catch(WorkplaceEmptyStorageException workplaceEmptyStorageException)
            {
                throw CreateAndLogValidationException(workplaceEmptyStorageException);
            }
            catch(SqlException sqlExcepton)
            {
                throw CreateAndLogCriticalDependencyException(sqlExcepton);
            }
            catch(Exception exception)
            {
                throw CreateAndLogServiceException(exception);
            }

               
        }

        private WorkplaceValidationException CreateAndLogValidationException(Exception exception)
        {
            var workplaceValidationException = new WorkplaceValidationException(exception);
            //LOG
            return workplaceValidationException;
        }

        private WorkplaceServiceException CreateAndLogServiceException(Exception exception)
        {
            var workplaceServiceException = new WorkplaceServiceException(exception);
            //LOG
            return workplaceServiceException;
        }
        
        private WorkplaceDependencyException CreateAndLogCriticalDependencyException(Exception exception)
        {
            var workplaceDependencyException = new WorkplaceDependencyException(exception);
            //LOG
            return workplaceDependencyException;
        }
    }
}
