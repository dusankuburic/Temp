using Temp.Domain.Models.ModeratorGroups.Exceptions;

namespace Temp.Core.Auth.Moderators.Service;

public partial class ModeratorService
{
    public delegate Task<UpdateModeratorGroups.Response> ReturningUpdateModeratorGroupFunction();


    public async Task<UpdateModeratorGroups.Response> TryCatch(
        ReturningUpdateModeratorGroupFunction returningUpdateModeratorGroupFunction) {
        try {
            return await returningUpdateModeratorGroupFunction();
        } catch (NullModeratorGroupsException nullModeratorGroupsException) {
            throw CreateAndLogValidationException(nullModeratorGroupsException);
        } catch (SqlException sqlException) {
            throw CreateAndLogCriticalDependencyException(sqlException);
        } catch (Exception exception) {
            throw CreateAndLogServiceException(exception);
        }
    }


    private ModeratorGroupValidationException CreateAndLogValidationException(Exception exception) {
        var moderatorGroupValidationException = new ModeratorGroupValidationException(exception);
        //LOG
        return moderatorGroupValidationException;
    }
    private ModeratorGroupsServiceException CreateAndLogServiceException(Exception exception) {
        var moderatorGroupsServiceException = new ModeratorGroupsServiceException(exception);
        //LOG
        return moderatorGroupsServiceException;
    }

    private ModeratorGroupsDependencyException CreateAndLogCriticalDependencyException(Exception exception) {
        var moderatorGroupsDependencyException = new ModeratorGroupsDependencyException(exception);
        //LOG
        return moderatorGroupsDependencyException;
    }
}
