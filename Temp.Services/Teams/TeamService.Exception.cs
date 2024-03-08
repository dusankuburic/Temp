using Temp.Services.Teams.Exceptions;
using Temp.Services.Teams.Models.Commands;
using Temp.Services.Teams.Models.Queries;

namespace Temp.Services.Teams;

public partial class TeamService
{
    private delegate Task<CreateTeamResponse> ReturningCreateTeamFunction();
    private delegate Task<GetTeamResponse> ReturningGetTeamFunction();
    private delegate Task<GetUserTeamResponse> ReturningGetUserTeamFunction();
    private delegate Task<UpdateTeamResponse> ReturningUpdateFunction();
    private delegate Task<UpdateTeamStatusResponse> ReturningUpdateTeamStatusFunction();
    private delegate Task<GetFullTeamTreeResponse> ReturningTeamTreeFunction();
    private delegate Task<bool> ReturningTeamExistsFunction();

    private async Task<GetFullTeamTreeResponse> TryCatch(ReturningTeamTreeFunction returningTeamTreeFunction) {
        try {
            return await returningTeamTreeFunction();
        } catch (SqlException sqlException) {
            throw CreateAndLogCriticalDependencyException(sqlException);
        } catch (Exception exception) {
            throw CreateAndLogServiceException(exception);
        }
    }

    private async Task<CreateTeamResponse> TryCatch(ReturningCreateTeamFunction returningCreateTeamFunction) {
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

    private async Task<GetTeamResponse> TryCatch(ReturningGetTeamFunction returningGetTeamFunction) {
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


    private async Task<GetUserTeamResponse> TryCatch(ReturningGetUserTeamFunction returningGetUserTeamFunction) {
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

    private async Task<UpdateTeamResponse> TryCatch(ReturningUpdateFunction returningUpdateFunction) {
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

    private async Task<UpdateTeamStatusResponse> TryCatch(ReturningUpdateTeamStatusFunction returningUpdateTeamStatusFunction) {
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

    private async Task<bool> TryCatch(ReturningTeamExistsFunction returningTeamExistsFunction) {
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

