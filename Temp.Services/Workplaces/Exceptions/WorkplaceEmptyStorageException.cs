namespace Temp.Services.Workplaces.Exceptions;

public class WorkplaceEmptyStorageException : Exception
{
    public WorkplaceEmptyStorageException()
        : base("No Workplaces found in storage") { }
}
