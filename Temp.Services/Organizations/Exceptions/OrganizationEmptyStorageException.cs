using Temp.Services.Exceptions;

namespace Temp.Services.Organizations.Exceptions;

public class OrganizationEmptyStorageException : NotFoundException
{
    public OrganizationEmptyStorageException()
        : base("No Organizations found in storage") {
    }
}