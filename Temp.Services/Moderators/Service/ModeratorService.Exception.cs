using Temp.Domain.Models.ModeratorGroups.Exceptions;

namespace Temp.Services.Moderators.Service;

public partial class ModeratorService
{
    public delegate Task<UpdateModeratorGroupsResponse> ReturningUpdateModeratorGroupFunction();


    public async Task<UpdateModeratorGroupsResponse> TryCatch(
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

        return moderatorGroupValidationException;
    }
    private ModeratorGroupsServiceException CreateAndLogServiceException(Exception exception) {
        var moderatorGroupsServiceException = new ModeratorGroupsServiceException(exception);

        return moderatorGroupsServiceException;
    }

    private ModeratorGroupsDependencyException CreateAndLogCriticalDependencyException(Exception exception) {
        var moderatorGroupsDependencyException = new ModeratorGroupsDependencyException(exception);

        return moderatorGroupsDependencyException;
    }
}