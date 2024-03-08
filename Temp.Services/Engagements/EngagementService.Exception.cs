using Temp.Services.Engagements.Exceptions;
using Temp.Services.Engagements.Models.Commands;
using Temp.Services.Engagements.Models.Queries;

namespace Temp.Services.Engagements;

public partial class EngagementService
{
    private delegate Task<CreateEngagementResponse> ReturningCreateEngagementFunction();
    private delegate Task<List<GetUserEmployeeEngagementsResponse>> ReturningUserEmployeeEngagementsFunction();
    private delegate Task<List<GetEngagementsForEmployeeResponse>> ReturningEngagementsForEmployeeFunction();

    private async Task<CreateEngagementResponse> TryCatch(ReturningCreateEngagementFunction returningCreateEngagementFunction) {
        try {
            return await returningCreateEngagementFunction();
        } catch (NullEngagementException nullEngagementException) {
            throw CreateAndLogValidationException(nullEngagementException);
        } catch (SqlException sqlException) {
            throw CreateAndLogCriticalDependencyException(sqlException);
        } catch (Exception exception) {
            throw CreateAndLogServiceException(exception);
        }
    }

    private async Task<List<GetUserEmployeeEngagementsResponse>> TryCatch(ReturningUserEmployeeEngagementsFunction returningUserEmployeeEngagementsFunction) {
        try {
            return await returningUserEmployeeEngagementsFunction();
        } catch (NullEngagementException nullEngagementException) {
            throw CreateAndLogValidationException(nullEngagementException);
        } catch (InvalidEngagementException invalidEngagementException) {
            throw CreateAndLogValidationException(invalidEngagementException);
        } catch (DateRangeEngagementException dateRangeEngagementException) {
            throw CreateAndLogValidationException(dateRangeEngagementException);
        } catch (SqlException sqlException) {
            throw CreateAndLogCriticalDependencyException(sqlException);
        } catch (Exception exception) {
            throw CreateAndLogServiceException(exception);
        }
    }

    private async Task<List<GetEngagementsForEmployeeResponse>> TryCatch(ReturningEngagementsForEmployeeFunction returningEngagementsForEmployeeFunction) {
        try {
            return await returningEngagementsForEmployeeFunction();
        } catch (NullEngagementException nullEngagementException) {
            throw CreateAndLogValidationException(nullEngagementException);
        } catch (InvalidEngagementException invalidEngagementException) {
            throw CreateAndLogValidationException(invalidEngagementException);
        } catch (DateRangeEngagementException dateRangeEngagementException) {
            throw CreateAndLogValidationException(dateRangeEngagementException);
        } catch (SqlException sqlException) {
            throw CreateAndLogCriticalDependencyException(sqlException);
        } catch (Exception exception) {
            throw CreateAndLogServiceException(exception);
        }
    }

    private EngagementServiceException CreateAndLogServiceException(Exception exception) {
        var engagementServiceException = new EngagementServiceException(exception);
        _loggingBroker.LogError(engagementServiceException);
        return engagementServiceException;
    }

    private EngagementValidationException CreateAndLogValidationException(Exception exception) {
        var engagementValidationException = new EngagementValidationException(exception);
        _loggingBroker.LogError(engagementValidationException);
        return engagementValidationException;
    }

    private EngagementDependencyException CreateAndLogCriticalDependencyException(Exception exception) {
        var engagementDependencyException = new EngagementDependencyException(exception);
        _loggingBroker.LogCritical(engagementDependencyException);
        return engagementDependencyException;
    }
}
