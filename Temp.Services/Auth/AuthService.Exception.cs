﻿using Temp.Domain.Models.Identity;
using Temp.Services.Auth.Exceptions;
using Temp.Services.Auth.Models.Commands;

namespace Temp.Services.Auth;

public partial class AuthService
{
    public delegate Task<LoginAppUserResponse> ReturningRegisterAppUserFunction();
    public delegate Task<AppUser> ReturningAppUserFunction();


    private async Task<LoginAppUserResponse> TryCatch(ReturningRegisterAppUserFunction returningRegisterAppUserFunction) {
        try {
            return await returningRegisterAppUserFunction();
        } catch (NullUserException nullUserException) {
            throw CreateAndLogValidationException(nullUserException);
        } catch (InvalidUserException invalidUserException) {
            throw CreateAndLogValidationException(invalidUserException);
        } catch (SqlException sqlException) {
            throw CreateAndLogCriticalDependencyException(sqlException);
        } catch (Exception exception) {
            throw CreateAndLogServiceException(exception);
        }
    }

    private async Task<AppUser> TryCatch(ReturningAppUserFunction returningAppUserFunction) {
        try {
            return await returningAppUserFunction();
        } catch (NullUserException nullUserException) {
            throw CreateAndLogValidationException(nullUserException);
        } catch (InvalidUserException invalidUserException) {
            throw CreateAndLogValidationException(invalidUserException);
        } catch (SqlException sqlException) {
            throw CreateAndLogCriticalDependencyException(sqlException);
        } catch (Exception exception) {
            throw CreateAndLogServiceException(exception);
        }
    }

    private UserServiceException CreateAndLogServiceException(Exception exception) {
        var userServiceException = new UserServiceException(exception);
        _loggingBroker.LogError(userServiceException);
        return userServiceException;
    }

    private UserValidationException CreateAndLogValidationException(Exception exception) {
        var userValidationException = new UserValidationException(exception);
        _loggingBroker.LogError(userValidationException);
        return userValidationException;
    }

    private UserDependencyException CreateAndLogCriticalDependencyException(Exception exception) {
        var userDependencyException = new UserDependencyException(exception);
        _loggingBroker.LogError(userDependencyException);
        return userDependencyException;
    }
}