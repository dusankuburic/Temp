using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Temp.Domain.Models.Applications.Exceptions;

namespace Temp.Core.Applications.Service
{
    public partial class ApplicationService
    {
        public delegate Task<CreateApplication.Response> ReturningCreateApplicationFunction();


        public async Task<CreateApplication.Response> TryCatch(ReturningCreateApplicationFunction returningCreateApplicationFunction)
        {
            try
            {
                return await returningCreateApplicationFunction();
            }
            catch(NullApplicationException nullApplicationException)
            {
                throw CreateAndLogValidationException(nullApplicationException);
            }
            catch(InvalidApplicationException invalidApplicationException)
            {
                throw CreateAndLogValidationException(invalidApplicationException);
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

       
        private ApplicationServiceException CreateAndLogServiceException(Exception exception)
        {
            var applicationServiceException = new ApplicationServiceException(exception);
            //LOG
            return applicationServiceException;
        }

        private ApplicationValidationException CreateAndLogValidationException(Exception exception)
        {
            var applicationValidationException = new ApplicationValidationException(exception);
            //LOG
            return applicationValidationException;
        }

        private ApplicationDependencyException CreateAndLogCriticalDependencyException(Exception exception)
        {
            var applicationDependencyException = new ApplicationDependencyException(exception);
            //LOG
            return applicationDependencyException;
        }

        
    }
}
