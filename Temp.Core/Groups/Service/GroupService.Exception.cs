using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using Temp.Domain.Models.Groups.Exceptions;

namespace Temp.Core.Groups.Service;

public partial class GroupService
{
    public delegate Task<CreateGroup.Response> ReturningCreateGroupFunction();
    public delegate Task<GetGroup.GroupViewModel> ReturningGetGroupFunction();
    public delegate Task<UpdateGroup.Response> ReturningUpdateGroupFunction();
    public delegate Task<IEnumerable<GetModeratorGroups.ModeratorGroupViewModel>> ReturningGetModeratorGroupsFunction();
    public delegate Task<IEnumerable<GetModeratorFreeGroups.ModeratorFreeGroupViewModel>> ReturningGetModeratorFreeGroupsFunction();
    public delegate Task<string> ReturningInnerTeamsFunction();


    public async Task<CreateGroup.Response> TryCatch(ReturningCreateGroupFunction returningCreateGroupFunction) {
        try {
            return await returningCreateGroupFunction();
        } catch (NullGroupException nullGroupException) {
            throw CreateAndLogValidationException(nullGroupException);
        } catch (InvalidGroupException invalidGroupException) {
            throw CreateAndLogValidationException(invalidGroupException);
        } catch (SqlException sqlExcepton) {
            throw CreateAndLogCriticalDependencyException(sqlExcepton);
        } catch (Exception exception) {
            throw CreateAndLogServiceException(exception);
        }
    }

    public async Task<GetGroup.GroupViewModel> TryCatch(ReturningGetGroupFunction returningGetGroupFunction) {
        try {
            return await returningGetGroupFunction();
        } catch (NullGroupException nullGroupException) {
            throw CreateAndLogServiceException(nullGroupException);
        } catch (SqlException sqlExcepton) {
            throw CreateAndLogCriticalDependencyException(sqlExcepton);
        } catch (Exception exception) {
            throw CreateAndLogServiceException(exception);
        }
    }

    public async Task<UpdateGroup.Response> TryCatch(ReturningUpdateGroupFunction returningUpdateGroupFunction) {
        try {
            return await returningUpdateGroupFunction();
        } catch (NullGroupException nullGroupException) {
            throw CreateAndLogServiceException(nullGroupException);
        } catch (InvalidGroupException invalidGroupException) {
            throw CreateAndLogValidationException(invalidGroupException);
        } catch (SqlException sqlExcepton) {
            throw CreateAndLogCriticalDependencyException(sqlExcepton);
        } catch (Exception exception) {
            throw CreateAndLogServiceException(exception);
        }
    }

    public async Task<IEnumerable<GetModeratorGroups.ModeratorGroupViewModel>> TryCatch
        (ReturningGetModeratorGroupsFunction returningGetModeratorGroupsFunction) {
        try {
            return await returningGetModeratorGroupsFunction();
        } catch (ModeratorGroupsEmptyStorageException moderatorGroupsEmptyStorageException) {
            throw CreateAndLogValidationException(moderatorGroupsEmptyStorageException);
        } catch (SqlException sqlExcepton) {
            throw CreateAndLogCriticalDependencyException(sqlExcepton);
        } catch (Exception exception) {
            throw CreateAndLogServiceException(exception);
        }
    }


    public async Task<IEnumerable<GetModeratorFreeGroups.ModeratorFreeGroupViewModel>> TryCatch
        (ReturningGetModeratorFreeGroupsFunction returningGetModeratorFreeGroupsFunction) {
        try {
            return await returningGetModeratorFreeGroupsFunction();
        } catch (SqlException sqlExcepton) {
            throw CreateAndLogCriticalDependencyException(sqlExcepton);
        } catch (Exception exception) {
            throw CreateAndLogServiceException(exception);
        }
    }

    public async Task<string> TryCatch(ReturningInnerTeamsFunction returningInnerTeamsFunction) {
        try {
            return await returningInnerTeamsFunction();
        } catch (NullGroupInnerTeamsException nullGroupInnerTeamsException) {
            throw CreateAndLogValidationException(nullGroupInnerTeamsException);
        } catch (GroupInnerTeamsStorageException groupInnerTeamsStorageException) {
            throw CreateAndLogValidationException(groupInnerTeamsStorageException);
        } catch (SqlException sqlExcepton) {
            throw CreateAndLogCriticalDependencyException(sqlExcepton);
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
