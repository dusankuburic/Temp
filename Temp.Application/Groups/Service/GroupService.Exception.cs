using Microsoft.Data.SqlClient;
using System;
using System.Threading.Tasks;
using Temp.Domain.Models.Groups.Exceptions;

namespace Temp.Application.Groups.Service
{
    public partial class GroupService
    {
        public delegate Task<CreateGroup.Response> ReturningCreateGroupFunction();
        public delegate Task<GetGroup.GroupViewModel> ReturningGetGroupFunction();
        public delegate Task<UpdateGroup.Response> ReturningUpdateGroupFunction();


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

        public async Task<GetGroup.GroupViewModel> TryCatch(ReturningGetGroupFunction returningGetGroupFunction)
        {
            try
            {
                return await returningGetGroupFunction();
            }
            catch(NullGroupException nullGroupException)
            {
                throw CreateAndLogServiceException(nullGroupException);
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

        public async Task<UpdateGroup.Response> TryCatch(ReturningUpdateGroupFunction returningUpdateGroupFunction)
        {
            try
            {
                return await returningUpdateGroupFunction();
            }
            catch (NullGroupException nullGroupException)
            {
                throw CreateAndLogServiceException(nullGroupException);
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
