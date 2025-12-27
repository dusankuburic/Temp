using Temp.Services.Exceptions;

namespace Temp.Services.Applications.Exceptions;

public class ApplicationWithTeamStorageException : NotFoundException
{
    public ApplicationWithTeamStorageException()
        : base("No applications found in storage for chosen team") {
    }
}