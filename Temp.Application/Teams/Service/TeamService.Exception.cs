using Microsoft.Data.SqlClient;
using System;
using System.Threading.Tasks;
using Temp.Domain.Models.Teams.Exceptions;

namespace Temp.Application.Teams.Service
{
    public partial class TeamService
    {
        public delegate Task<CreateTeam.Response> ReturningCreateTeamFunction();
        public delegate GetTeam.TeamViewModel ReturningGetTeamFunction();
        public delegate Task<UpdateTeam.Response> ReturningUpateFunction();


        public async Task<CreateTeam.Response> TryCatch(ReturningCreateTeamFunction returningCreateTeamFunction)
        {
            try
            {
                return await returningCreateTeamFunction();
            }
            catch (NullTeamException nullTeamException)
            {
                throw CreateAndLogValidationException(nullTeamException);
            }
            catch (InvalidTeamException invalidTeamException)
            {
                throw CreateAndLogValidationException(invalidTeamException);
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


        public GetTeam.TeamViewModel TryCatch(ReturningGetTeamFunction returningGetTeamFunction)
        {
            try
            {
                return returningGetTeamFunction();
            }
            catch(NullTeamException nullTeamException)
            {
                throw CreateAndLogServiceException(nullTeamException);
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

        public async Task<UpdateTeam.Response> TryCatch(ReturningUpateFunction returningUpateFunction)
        {
            try
            {
                return await returningUpateFunction();
            }
            catch (NullTeamException nullTeamException)
            {
                throw CreateAndLogValidationException(nullTeamException);
            }
            catch (InvalidTeamException invalidTeamException)
            {
                throw CreateAndLogValidationException(invalidTeamException);
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

        private TeamValidationException CreateAndLogValidationException(Exception exception)
        {
            var teamValidationException = new TeamValidationException(exception);
            //LOG
            return teamValidationException;
        }

        private TeamServiceException CreateAndLogServiceException(Exception exception)
        {
            var teamServiceException = new TeamServiceException(exception);
            //LOG
            return teamServiceException;
        }

        private TeamDependencyException CreateAndLogCriticalDependencyException(Exception exception)
        {
            var teamDependencyException = new TeamDependencyException(exception);
            //LOG
            return teamDependencyException;
        }
    }
}
