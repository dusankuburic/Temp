namespace Temp.Domain.Models.ModeratorGroups.Exceptions;

public class ModeratorGroupValidationException : Exception
{
    public ModeratorGroupValidationException(Exception innerException)
        : base("Invalid input, contact support", innerException) {

    }
}