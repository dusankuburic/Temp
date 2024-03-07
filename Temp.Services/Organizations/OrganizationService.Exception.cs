﻿using Microsoft.Data.SqlClient;
using Temp.Services._Helpers;
using Temp.Services.Organizations.Exceptions;
using Temp.Services.Organizations.Models.Commands;
using Temp.Services.Organizations.Models.Queries;

namespace Temp.Services.Organizations;

public partial class OrganizationService
{
    public delegate Task<CreateOrganizationResponse> ReturningCreateOrganizationFunction();
    public delegate Task<PagedList<GetOrganizationResponse>> ReturningGetPagedOrganizationsFunction();
    public delegate Task<IEnumerable<GetOrganizationResponse>> ReturningGetOrganizationsFunction();
    public delegate Task<GetOrganizationResponse> ReturningGetOrganizationFunction();
    public delegate Task<UpdateOrganizationResponse> ReturningUpdateOrganizationFunction();
    public delegate Task<UpdateOrganizationStatusResponse> ReturningUpdateOrganizationStatusFunction();
    public delegate Task<GetPagedInnerGroupsResponse> ReturningGetPagedInnerGroupsFunction();
    public delegate Task<List<InnerGroup>> ReturningGetInnerGroupsFunction();
    public delegate Task<bool> ReturningOrganizationExistsFunction();

    public async Task<CreateOrganizationResponse> TryCatch(ReturningCreateOrganizationFunction returningCreateOrganizationFunction) {
        try {
            return await returningCreateOrganizationFunction();
        } catch (NullOrganizationException nullOrganizationException) {
            throw CreateAndLogValidationException(nullOrganizationException);
        } catch (InvalidOrganizationException invalidOrganizationException) {
            throw CreateAndLogValidationException(invalidOrganizationException);
        } catch (SqlException sqlException) {
            throw CreateAndLogCriticalDependencyException(sqlException);
        } catch (Exception exception) {
            throw CreateAndLogServiceException(exception);
        }
    }

    public async Task<GetPagedInnerGroupsResponse> TryCatch(ReturningGetPagedInnerGroupsFunction returningGetPagedInnerGroupsFunction) {
        try {
            return await returningGetPagedInnerGroupsFunction();
        } catch (SqlException sqlException) {
            throw CreateAndLogValidationException(sqlException);
        } catch (Exception exception) {
            throw CreateAndLogServiceException(exception);
        }
    }

    public async Task<PagedList<GetOrganizationResponse>> TryCatch(ReturningGetPagedOrganizationsFunction returningGetPagedOrganizationsFunction) {
        try {
            return await returningGetPagedOrganizationsFunction();
        } catch (SqlException sqlException) {
            throw CreateAndLogValidationException(sqlException);
        } catch (Exception exception) {
            throw CreateAndLogServiceException(exception);
        }
    }

    public async Task<IEnumerable<GetOrganizationResponse>> TryCatch(ReturningGetOrganizationsFunction returningGetOrganizationsFunction) {
        try {
            return await returningGetOrganizationsFunction();
        } catch (OrganizationEmptyStorageException organizationEmptyStorageException) {
            throw CreateAndLogValidationException(organizationEmptyStorageException);
        } catch (SqlException sqlException) {
            throw CreateAndLogCriticalDependencyException(sqlException);
        } catch (Exception exception) {
            throw CreateAndLogServiceException(exception);
        }
    }

    public async Task<GetOrganizationResponse> TryCatch(ReturningGetOrganizationFunction returningGetOrganizationFunction) {
        try {
            return await returningGetOrganizationFunction();
        } catch (NullOrganizationException nullOrganizationException) {
            throw CreateAndLogServiceException(nullOrganizationException);
        } catch (SqlException sqlException) {
            throw CreateAndLogCriticalDependencyException(sqlException);
        } catch (Exception exception) {
            throw CreateAndLogServiceException(exception);
        }
    }

    public async Task<UpdateOrganizationResponse> TryCatch(ReturningUpdateOrganizationFunction returningUpdateOrganizationFunction) {
        try {
            return await returningUpdateOrganizationFunction();
        } catch (NullOrganizationException nullOrganizationException) {
            throw CreateAndLogValidationException(nullOrganizationException);
        } catch (InvalidOrganizationException invalidOrganizationException) {
            throw CreateAndLogValidationException(invalidOrganizationException);
        } catch (SqlException sqlException) {
            throw CreateAndLogCriticalDependencyException(sqlException);
        } catch (Exception exception) {
            throw CreateAndLogServiceException(exception);
        }
    }

    public async Task<UpdateOrganizationStatusResponse> TryCatch(ReturningUpdateOrganizationStatusFunction returningUpdateOrganizationStatusFunction) {
        try {
            return await returningUpdateOrganizationStatusFunction();
        } catch (NullOrganizationException nullOrganizationException) {
            throw CreateAndLogValidationException(nullOrganizationException);
        } catch (InvalidOrganizationException invalidOrganizationException) {
            throw CreateAndLogValidationException(invalidOrganizationException);
        } catch (SqlException sqlException) {
            throw CreateAndLogCriticalDependencyException(sqlException);
        } catch (Exception exception) {
            throw CreateAndLogServiceException(exception);
        }
    }

    public async Task<List<InnerGroup>> TryCatch(ReturningGetInnerGroupsFunction returningGetInnerGroupsFunction) {
        try {
            return await returningGetInnerGroupsFunction();
        } catch (OrganizationGetInnerGroupsStorageException organizationGetInnerGroupsStorageException) {
            throw CreateAndLogServiceException(organizationGetInnerGroupsStorageException);
        } catch (SqlException sqlException) {
            throw CreateAndLogCriticalDependencyException(sqlException);
        } catch (Exception exception) {
            throw CreateAndLogServiceException(exception);
        }
    }

    public async Task<bool> TryCatch(ReturningOrganizationExistsFunction returningOrganizationExistsFunction) {
        try {
            return await returningOrganizationExistsFunction();
        } catch (SqlException sqlException) {
            throw CreateAndLogCriticalDependencyException(sqlException);
        } catch (Exception exception) {
            throw CreateAndLogServiceException(exception);
        }
    }

    private OrganizationValidationException CreateAndLogValidationException(Exception exception) {
        var organizationValidationException = new OrganizationValidationException(exception);
        _loggingBroker.LogError(organizationValidationException);
        return organizationValidationException;
    }

    private OrganizationServiceException CreateAndLogServiceException(Exception exception) {
        var organizationServiceException = new OrganizationServiceException(exception);
        _loggingBroker.LogError(organizationServiceException);
        return organizationServiceException;
    }

    private OrganizationDependencyException CreateAndLogCriticalDependencyException(Exception exception) {
        var organizationDependencyException = new OrganizationDependencyException(exception);
        _loggingBroker.LogCritical(organizationDependencyException);
        return organizationDependencyException;
    }
}

