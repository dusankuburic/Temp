using Temp.Services.Exceptions;

namespace Temp.Services.Auth.Exceptions;

public class UserServiceException : ServiceException
{
    public UserServiceException(Exception innerException)
        : base("User service error occurred", "USER_SERVICE_ERROR", innerException) {
    }

    public UserServiceException(string message, Exception innerException)
        : base(message, "USER_SERVICE_ERROR", innerException) {
    }
}