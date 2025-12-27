using Temp.Services.Exceptions;

namespace Temp.Services.Applications.Exceptions;

public class ApplicationWithUserStorageException : NotFoundException
{
    public ApplicationWithUserStorageException()
        : base("No applications found in storage for chosen user") {
    }
}