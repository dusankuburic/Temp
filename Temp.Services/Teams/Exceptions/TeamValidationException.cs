namespace Temp.Services.Teams.Exceptions;

public class TeamValidationException : Exception
{
    public TeamValidationException(Exception innerException)
        : base("Invalid input, contact support", innerException) {

    }
}

