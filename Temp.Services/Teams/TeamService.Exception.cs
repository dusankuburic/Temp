using Microsoft.Data.SqlClient;
using Temp.Services.Teams.Exceptions;
using Temp.Services.Teams.Models.Command;
using Temp.Services.Teams.Models.Query;

namespace Temp.Services.Teams;

public partial class TeamService
{
    public delegate Task<CreateTeam.Response> ReturningCreateTeamFunction();
    public delegate Task<GetTeam.TeamViewModel> ReturningGetTeamFunction();
    public delegate Task<GetUserTeam.TeamViewModel> ReturningGetUserTeamFunction();
    public delegate Task<UpdateTeam.Response> ReturningUpdateFunction();
    public delegate Task<GetFullTeamTree.TeamTreeViewModel> ReturningTeamTreeFunction();


    public async Task<GetFullTeamTree.TeamTreeViewModel> TryCatch(ReturningTeamTreeFunction returningTeamTreeFunction) {
        try {
            return await returningTeamTreeFunction();
        } catch (SqlException sqlException) {
            throw CreateAndLogCriticalDependencyException(sqlException);
        } catch (Exception exception) {
            throw CreateAndLogServiceException(exception);
        }
    }

    public async Task<CreateTeam.Response> TryCatch(ReturningCreateTeamFunction returningCreateTeamFunction) {
        try {
            return await returningCreateTeamFunction();
        } catch (NullTeamException nullTeamException) {
            throw CreateAndLogValidationException(nullTeamException);
        } catch (InvalidTeamException invalidTeamException) {
            throw CreateAndLogValidationException(invalidTeamException);
        } catch (SqlException sqlException) {
            throw CreateAndLogCriticalDependencyException(sqlException);
        } catch (Exception exception) {
            throw CreateAndLogServiceException(exception);
        }
    }

    public async Task<GetTeam.TeamViewModel> TryCatch(ReturningGetTeamFunction returningGetTeamFunction) {
        try {
            return await returningGetTeamFunction();
        } catch (NullTeamException nullTeamException) {
            throw CreateAndLogServiceException(nullTeamException);
        } catch (SqlException sqlException) {
            throw CreateAndLogCriticalDependencyException(sqlException);
        } catch (Exception exception) {
            throw CreateAndLogServiceException(exception);
        }
    }


    public async Task<GetUserTeam.TeamViewModel> TryCatch(ReturningGetUserTeamFunction returningGetUserTeamFunction) {
        try {
            return await returningGetUserTeamFunction();
        } catch (NullTeamException nullTeamException) {
            throw CreateAndLogServiceException(nullTeamException);
        } catch (SqlException sqlException) {
            throw CreateAndLogCriticalDependencyException(sqlException);
        } catch (Exception exception) {
            throw CreateAndLogServiceException(exception);
        }
    }

    public async Task<UpdateTeam.Response> TryCatch(ReturningUpdateFunction returningUpdateFunction) {
        try {
            return await returningUpdateFunction();
        } catch (NullTeamException nullTeamException) {
            throw CreateAndLogValidationException(nullTeamException);
        } catch (InvalidTeamException invalidTeamException) {
            throw CreateAndLogValidationException(invalidTeamException);
        } catch (SqlException sqlException) {
            throw CreateAndLogCriticalDependencyException(sqlException);
        } catch (Exception exception) {
            throw CreateAndLogServiceException(exception);
        }
    }

    private TeamValidationException CreateAndLogValidationException(Exception exception) {
        var teamValidationException = new TeamValidationException(exception);
        //LOG
        return teamValidationException;
    }

    private TeamServiceException CreateAndLogServiceException(Exception exception) {
        var teamServiceException = new TeamServiceException(exception);
        //LOG
        return teamServiceException;
    }

    private TeamDependencyException CreateAndLogCriticalDependencyException(Exception exception) {
        var teamDependencyException = new TeamDependencyException(exception);
        //LOG
        return teamDependencyException;
    }
}

