using Microsoft.Data.SqlClient;
using Temp.Services.Teams.Exceptions;
using Temp.Services.Teams.Models.Commands;
using Temp.Services.Teams.Models.Queries;

namespace Temp.Services.Teams;

public partial class TeamService
{
    public delegate Task<CreateTeamResponse> ReturningCreateTeamFunction();
    public delegate Task<GetTeamResponse> ReturningGetTeamFunction();
    public delegate Task<GetUserTeamResponse> ReturningGetUserTeamFunction();
    public delegate Task<UpdateTeamResponse> ReturningUpdateFunction();
    public delegate Task<UpdateTeamStatusResponse> ReturningUpdateTeamStatusFunction();
    public delegate Task<GetFullTeamTreeResponse> ReturningTeamTreeFunction();
    public delegate Task<bool> ReturningTeamExistsFunction();

    public async Task<GetFullTeamTreeResponse> TryCatch(ReturningTeamTreeFunction returningTeamTreeFunction) {
        try {
            return await returningTeamTreeFunction();
        } catch (SqlException sqlException) {
            throw CreateAndLogCriticalDependencyException(sqlException);
        } catch (Exception exception) {
            throw CreateAndLogServiceException(exception);
        }
    }

    public async Task<CreateTeamResponse> TryCatch(ReturningCreateTeamFunction returningCreateTeamFunction) {
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

    public async Task<GetTeamResponse> TryCatch(ReturningGetTeamFunction returningGetTeamFunction) {
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


    public async Task<GetUserTeamResponse> TryCatch(ReturningGetUserTeamFunction returningGetUserTeamFunction) {
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

    public async Task<UpdateTeamResponse> TryCatch(ReturningUpdateFunction returningUpdateFunction) {
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

    public async Task<UpdateTeamStatusResponse> TryCatch(ReturningUpdateTeamStatusFunction returningUpdateTeamStatusFunction) {
        try {
            return await returningUpdateTeamStatusFunction();
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

    public async Task<bool> TryCatch(ReturningTeamExistsFunction returningTeamExistsFunction) {
        try {
            return await returningTeamExistsFunction();
        } catch (SqlException sqlException) {
            throw CreateAndLogCriticalDependencyException(sqlException);
        } catch (Exception exception) {
            throw CreateAndLogServiceException(exception);
        }
    }

    private TeamValidationException CreateAndLogValidationException(Exception exception) {
        var teamValidationException = new TeamValidationException(exception);
        _loggingBroker.LogError(teamValidationException);
        return teamValidationException;
    }

    private TeamServiceException CreateAndLogServiceException(Exception exception) {
        var teamServiceException = new TeamServiceException(exception);
        _loggingBroker.LogError(teamServiceException);
        return teamServiceException;
    }

    private TeamDependencyException CreateAndLogCriticalDependencyException(Exception exception) {
        var teamDependencyException = new TeamDependencyException(exception);
        _loggingBroker.LogCritical(teamDependencyException);
        return teamDependencyException;
    }
}

