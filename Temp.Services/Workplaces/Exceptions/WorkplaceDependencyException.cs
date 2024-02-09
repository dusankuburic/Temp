namespace Temp.Services.Workplaces.Exceptions;

public class WorkplaceDependencyException : Exception
{
    public WorkplaceDependencyException(Exception innerException)
        : base("Service dependency error occurred, contact support", innerException) { }
}