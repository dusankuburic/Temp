using Temp.Services.Groups.Exceptions;
using Temp.Services.Groups.Models.Commands;
using Temp.Services.Groups.Models.Queries;

namespace Temp.Services.Groups;

public partial class GroupService
{
    private delegate Task<CreateGroupResponse> ReturningCreateGroupFunction();
    private delegate Task<GetGroupResponse> ReturningGetGroupFunction();
    private delegate Task<UpdateGroupResponse> ReturningUpdateGroupFunction();
    private delegate Task<UpdateGroupStatusResponse> ReturningUpdateGroupStatusFunction();
    private delegate Task<GetPagedGroupInnerTeamsResponse> ReturningPagedGroupInnerTeamsFunction();
    private delegate Task<List<InnerTeam>> ReturningGroupInnerTeamsFunction();
    private delegate Task<List<GetModeratorGroupsResponse>> ReturningModeratorGroupsFunction();
    private delegate Task<List<GetModeratorFreeGroupsResponse>> ReturningModeratorFreeGroupsFunction();
    private delegate Task<bool> ReturningGroupExistsFunction();

    private async Task<CreateGroupResponse> TryCatch(ReturningCreateGroupFunction returningCreateGroupFunction) {
        try {
            return await returningCreateGroupFunction();
        } catch (NullGroupException nullGroupException) {
            throw CreateAndLogValidationException(nullGroupException);
        } catch (InvalidGroupException invalidGroupException) {
            throw CreateAndLogValidationException(invalidGroupException);
        } catch (SqlException sqlException) {
            throw CreateAndLogCriticalDependencyException(sqlException);
        } catch (Exception exception) {
            throw CreateAndLogServiceException(exception);
        }
    }

    private async Task<GetGroupResponse> TryCatch(ReturningGetGroupFunction returningGetGroupFunction) {
        try {
            return await returningGetGroupFunction();
        } catch (NullGroupException nullGroupException) {
            throw CreateAndLogServiceException(nullGroupException);
        } catch (SqlException sqlException) {
            throw CreateAndLogCriticalDependencyException(sqlException);
        } catch (Exception exception) {
            throw CreateAndLogServiceException(exception);
        }
    }

    private async Task<UpdateGroupResponse> TryCatch(ReturningUpdateGroupFunction returningUpdateGroupFunction) {
        try {
            return await returningUpdateGroupFunction();
        } catch (NullGroupException nullGroupException) {
            throw CreateAndLogServiceException(nullGroupException);
        } catch (InvalidGroupException invalidGroupException) {
            throw CreateAndLogValidationException(invalidGroupException);
        } catch (SqlException sqlException) {
            throw CreateAndLogCriticalDependencyException(sqlException);
        } catch (Exception exception) {
            throw CreateAndLogServiceException(exception);
        }
    }

    private async Task<UpdateGroupStatusResponse> TryCatch(ReturningUpdateGroupStatusFunction returningUpdateGroupStatusFunction) {
        try {
            return await returningUpdateGroupStatusFunction();
        } catch (NullGroupException nullGroupException) {
            throw CreateAndLogServiceException(nullGroupException);
        } catch (InvalidGroupException invalidGroupException) {
            throw CreateAndLogValidationException(invalidGroupException);
        } catch (SqlException sqlException) {
            throw CreateAndLogCriticalDependencyException(sqlException);
        } catch (Exception exception) {
            throw CreateAndLogServiceException(exception);
        }
    }

    private async Task<GetPagedGroupInnerTeamsResponse> TryCatch(ReturningPagedGroupInnerTeamsFunction returningPagedGroupInnerTeamsFunction) {
        try {
            return await returningPagedGroupInnerTeamsFunction();
        } catch (SqlException sqlException) {
            throw CreateAndLogCriticalDependencyException(sqlException);
        } catch (Exception exception) {
            throw CreateAndLogServiceException(exception);
        }
    }

    private async Task<List<InnerTeam>> TryCatch(ReturningGroupInnerTeamsFunction returningGroupInnerTeamsFunction) {
        try {
            return await returningGroupInnerTeamsFunction();
        } catch (SqlException sqlException) {
            throw CreateAndLogCriticalDependencyException(sqlException);
        } catch (Exception exception) {
            throw CreateAndLogServiceException(exception);
        }
    }

    private async Task<List<GetModeratorGroupsResponse>> TryCatch(ReturningModeratorGroupsFunction returningModeratorGroupsFunction) {
        try {
            return await returningModeratorGroupsFunction();
        } catch (SqlException sqlException) {
            throw CreateAndLogCriticalDependencyException(sqlException);
        } catch (Exception exception) {
            throw CreateAndLogServiceException(exception);
        }
    }

    private async Task<List<GetModeratorFreeGroupsResponse>> TryCatch(ReturningModeratorFreeGroupsFunction returningModeratorFreeGroupsFunction) {
        try {
            return await returningModeratorFreeGroupsFunction();
        } catch (SqlException sqlException) {
            throw CreateAndLogCriticalDependencyException(sqlException);
        } catch (Exception exception) {
            throw CreateAndLogServiceException(exception);
        }
    }

    private async Task<bool> TryCatch(ReturningGroupExistsFunction returningGroupExistsFunction) {
        try {
            return await returningGroupExistsFunction();
        } catch (SqlException sqlException) {
            throw CreateAndLogCriticalDependencyException(sqlException);
        } catch (Exception exception) {
            throw CreateAndLogServiceException(exception);
        }
    }

    private GroupValidationException CreateAndLogValidationException(Exception exception) {
        var groupValidationException = new GroupValidationException(exception);
        Logger.LogError(groupValidationException);
        return groupValidationException;
    }

    private GroupServiceException CreateAndLogServiceException(Exception exception) {
        var groupServiceException = new GroupServiceException(exception);
        Logger.LogError(groupServiceException);
        return groupServiceException;
    }

    private GroupDependencyException CreateAndLogCriticalDependencyException(Exception exception) {
        var groupDependencyException = new GroupDependencyException(exception);
        Logger.LogCritical(groupDependencyException);
        return groupDependencyException;
    }
}