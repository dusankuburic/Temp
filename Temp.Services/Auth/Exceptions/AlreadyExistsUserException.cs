using Temp.Services.Exceptions;

namespace Temp.Services.Auth.Exceptions;

public class AlreadyExistsUserException : ConflictException
{
    public AlreadyExistsUserException()
        : base("A user with the same identifier already exists") {
    }

    public AlreadyExistsUserException(string userId)
        : base($"A user with identifier '{userId}' already exists") {
    }
}