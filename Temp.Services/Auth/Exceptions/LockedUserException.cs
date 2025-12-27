using ValidationEx = Temp.Services.Exceptions.ValidationException;

namespace Temp.Services.Auth.Exceptions;

public class LockedUserException : ValidationEx
{
    public LockedUserException()
        : base("User account is locked", "User", "User account is locked. Please try again later.") {
    }

    public LockedUserException(string userId)
        : base($"User account {userId} is locked", "User", "User account is locked. Please try again later.") {
    }
}