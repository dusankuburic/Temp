using Microsoft.Data.SqlClient;
using System;
using System.Threading.Tasks;
using Temp.Domain.Models.Engagements.Exceptions;

namespace Temp.Application.Engagements
{
    public partial class EngagementService
    {
        public delegate Task<CreateEngagement.Response> ReturningCreateEngagementFunction();


        public async Task<CreateEngagement.Response> TryCatch(ReturningCreateEngagementFunction returningCreateEngagementFunction)
        {
            try
            {
                return await returningCreateEngagementFunction();
            }    
            catch(NullEngagementException nullEngagementException)
            {
                throw CreateAndLogValidationException(nullEngagementException);
            }
            catch(InvalidEngagementException invalidEngagementException)
            {
                throw CreateAndLogValidationException(invalidEngagementException);
            }
            catch(DateRangeEngagementException dateRangeEngagementException)
            {
                throw CreateAndLogValidationException(dateRangeEngagementException);
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


        private EngagementServiceException CreateAndLogServiceException(Exception exception)
        {
            var engagementServiceException = new EngagementServiceException(exception);
            //LOG
            return engagementServiceException;
        }

        private EngagementValidationException CreateAndLogValidationException(Exception exception)
        {
            var engagementValidationException = new EngagementValidationException(exception);
            //LOG
            return engagementValidationException;
        }

        private EngagementDependencyException CreateAndLogCriticalDependencyException(Exception exception)
        {
            var engagementDependencyException = new EngagementDependencyException(exception);
            //LOG
            return engagementDependencyException;
        }

    }
}
