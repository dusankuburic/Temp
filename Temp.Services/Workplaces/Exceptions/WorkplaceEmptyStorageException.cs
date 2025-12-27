using Temp.Services.Exceptions;

namespace Temp.Services.Workplaces.Exceptions;

public class WorkplaceEmptyStorageException : NotFoundException
{
    public WorkplaceEmptyStorageException()
        : base("No Workplaces found in storage") {
    }
}