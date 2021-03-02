using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Temp.Application.Helpers;
using Temp.Domain.Models.Workplaces.Exceptions;

namespace Temp.Application.Workplaces.Service
{
    public partial class WorkplaceService
    {
        public delegate Task<CreateWorkplace.Response> ReturningWorkplaceFunction();
        public delegate IEnumerable<GetWorkplaces.WorkplacesViewModel> ReturningGetWorkplacesFunction();
        public delegate Task<PagedList<GetWorkplaces.WorkplacesViewModel>> ReturningGetWorkplacesFunctionPage();
        public delegate GetWorkplace.WorkplaceViewModel ReturningGetWorkplaceFunction();
        public delegate Task<UpdateWorkplace.Response> ReturningUpdateWorkplaceFunction();

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
            catch(SqlException sqlException)
            {
                throw CreateAndLogCriticalDependencyException(sqlException);
            }
            catch(Exception exception)
            {
                throw CreateAndLogServiceException(exception);
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

        public async Task<PagedList<GetWorkplaces.WorkplacesViewModel>> TryCatch(ReturningGetWorkplacesFunctionPage returningGetWorkplacesFunctionPage)
        {
            try
            {
                return await returningGetWorkplacesFunctionPage();
            }
            catch(WorkplaceEmptyStorageException workplaceEmptyStorageException)
            {
                throw CreateAndLogValidationException(workplaceEmptyStorageException);
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

        public GetWorkplace.WorkplaceViewModel TryCatch(ReturningGetWorkplaceFunction returningGetWorkplaceFunction)
        {
            try
            {
                return returningGetWorkplaceFunction();
            }
            catch(NullWorkplaceException nullWorkplaceException)
            {
                throw CreateAndLogValidationException(nullWorkplaceException);
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

        public async Task<UpdateWorkplace.Response> TryCatch(ReturningUpdateWorkplaceFunction returningUpdateWorkplaceFunction)
        {
            try
            {
                return await returningUpdateWorkplaceFunction();
            }
            catch(NullWorkplaceException nullWorkplaceException)
            {
                throw CreateAndLogValidationException(nullWorkplaceException);
            }
            catch(InvalidWorkplaceException invalidWorkplaceException)
            {
                throw CreateAndLogValidationException(invalidWorkplaceException);
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
