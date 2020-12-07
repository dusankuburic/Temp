using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Temp.Domain.Models.Groups.Exceptions;

namespace Temp.Application.Groups.Service
{
    public partial class GroupService
    {
        public delegate Task<CreateGroup.Response> ReturningCreateGroupFunction();


        public async Task<CreateGroup.Response> TryCatch(ReturningCreateGroupFunction returningCreateGroupFunction)
        {
            try
            {
                return await returningCreateGroupFunction();
            }
            catch(NullGroupException nullGroupException)
            {
                throw CreateAndLogValidationException(nullGroupException);
            }
            catch(InvalidGroupException invalidGroupException)
            {
                throw CreateAndLogValidationException(invalidGroupException);
            }
            catch (SqlException sqlExcepton)
            {
                throw CreateAndLogCriticalDependencyException(sqlExcepton);
            }
            catch (Exception exception)
            {
                throw CreateAndLogServiceException(exception);
            }
        }


        private GroupValidationException CreateAndLogValidationException(Exception exception)
        {
            var groupValidationException = new GroupValidationException(exception);
            //LOG
            return groupValidationException;
        }

        private GroupServiceException CreateAndLogServiceException(Exception exception)
        {
            var groupServiceException = new GroupServiceException(exception);
            //LOG
            return groupServiceException;
        }

        private GroupDependencyException CreateAndLogCriticalDependencyException(Exception exception)
        {
            var groupDependencyException = new GroupDependencyException(exception);
            //LOG
            return groupDependencyException;
        }
    }
}
