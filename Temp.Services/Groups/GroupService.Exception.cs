using Microsoft.Data.SqlClient;
using Temp.Services.Groups.Exceptions;
using Temp.Services.Groups.Models.Commands;
using Temp.Services.Groups.Models.Queries;

namespace Temp.Services.Groups;

public partial class GroupService
{
    public delegate Task<CreateGroupResponse> ReturningCreateGroupFunction();
    public delegate Task<GetGroupResponse> ReturningGetGroupFunction();
    public delegate Task<UpdateGroupResponse> ReturningUpdateGroupFunction();
    public delegate Task<UpdateGroupStatusResponse> ReturningUpdateGroupStatusFunction();
    public delegate Task<GetGroupInnerTeamsResponse> ReturningGroupInnerTeamsFunction();
    public delegate Task<List<GetModeratorGroupsResponse>> ReturningModeratorGroupsFunction();
    public delegate Task<List<GetModeratorFreeGroupsResponse>> ReturningModeratorFreeGroupsFunction();

    public async Task<CreateGroupResponse> TryCatch(ReturningCreateGroupFunction returningCreateGroupFunction) {
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

    public async Task<GetGroupResponse> TryCatch(ReturningGetGroupFunction returningGetGroupFunction) {
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

    public async Task<UpdateGroupResponse> TryCatch(ReturningUpdateGroupFunction returningUpdateGroupFunction) {
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

    public async Task<UpdateGroupStatusResponse> TryCatch(ReturningUpdateGroupStatusFunction returningUpdateGroupStatusFunction) {
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

    public async Task<GetGroupInnerTeamsResponse> TryCatch(ReturningGroupInnerTeamsFunction returningGroupInnerTeamsFunction) {
        try {
            return await returningGroupInnerTeamsFunction();
        } catch (SqlException sqlException) {
            throw CreateAndLogCriticalDependencyException(sqlException);
        } catch (Exception exception) {
            throw CreateAndLogServiceException(exception);
        }
    }

    public async Task<List<GetModeratorGroupsResponse>> TryCatch(ReturningModeratorGroupsFunction returningModeratorGroupsFunction) {
        try {
            return await returningModeratorGroupsFunction();
        } catch (SqlException sqlException) {
            throw CreateAndLogCriticalDependencyException(sqlException);
        } catch (Exception exception) {
            throw CreateAndLogServiceException(exception);
        }
    }

    public async Task<List<GetModeratorFreeGroupsResponse>> TryCatch(ReturningModeratorFreeGroupsFunction returningModeratorFreeGroupsFunction) {
        try {
            return await returningModeratorFreeGroupsFunction();
        } catch (SqlException sqlException) {
            throw CreateAndLogCriticalDependencyException(sqlException);
        } catch (Exception exception) {
            throw CreateAndLogServiceException(exception);
        }
    }

    private GroupValidationException CreateAndLogValidationException(Exception exception) {
        var groupValidationException = new GroupValidationException(exception);
        //LOG
        return groupValidationException;
    }

    private GroupServiceException CreateAndLogServiceException(Exception exception) {
        var groupServiceException = new GroupServiceException(exception);
        //LOG
        return groupServiceException;
    }

    private GroupDependencyException CreateAndLogCriticalDependencyException(Exception exception) {
        var groupDependencyException = new GroupDependencyException(exception);
        //LOG
        return groupDependencyException;
    }
}
