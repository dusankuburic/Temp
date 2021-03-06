﻿using Microsoft.Data.SqlClient;
using System;
using System.Threading.Tasks;
using Temp.Domain.Models.Teams.Exceptions;

namespace Temp.Application.Teams.Service
{
    public partial class TeamService
    {
        public delegate Task<CreateTeam.Response> ReturningCreateTeamFunction();
        public delegate Task<GetTeam.TeamViewModel> ReturningGetTeamFunction();
        public delegate Task<UpdateTeam.Response> ReturningUpdateFunction();
        public delegate Task<GetFullTeamTree.TeamTreeViewModel> ReturningTeamTreeFunction();


        public async Task<GetFullTeamTree.TeamTreeViewModel> TryCatch(ReturningTeamTreeFunction returningTeamTreeFunction)
        {
            try
            {
                return await returningTeamTreeFunction();
            }
            catch (SqlException sqlException)
            {
                throw CreateAndLogCriticalDependencyException(sqlException);
            }
            catch (Exception exception)
            {
                throw CreateAndLogServiceException(exception);
            }
            
        }


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
            catch (SqlException sqlException)
            {
                throw CreateAndLogCriticalDependencyException(sqlException);
            }
            catch (Exception exception)
            {
                throw CreateAndLogServiceException(exception);
            }
        }

        public async Task<GetTeam.TeamViewModel> TryCatch(ReturningGetTeamFunction returningGetTeamFunction)
        {
            try
            {
                return await returningGetTeamFunction();
            }
            catch(NullTeamException nullTeamException)
            {
                throw CreateAndLogServiceException(nullTeamException);
            }
            catch (SqlException sqlException)
            {
                throw CreateAndLogCriticalDependencyException(sqlException);
            }
            catch (Exception exception)
            {
                throw CreateAndLogServiceException(exception);
            }
        }

        public async Task<UpdateTeam.Response> TryCatch(ReturningUpdateFunction returningUpdateFunction)
        {
            try
            {
                return await returningUpdateFunction();
            }
            catch (NullTeamException nullTeamException)
            {
                throw CreateAndLogValidationException(nullTeamException);
            }
            catch (InvalidTeamException invalidTeamException)
            {
                throw CreateAndLogValidationException(invalidTeamException);
            }
            catch (SqlException sqlException)
            {
                throw CreateAndLogCriticalDependencyException(sqlException);
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
