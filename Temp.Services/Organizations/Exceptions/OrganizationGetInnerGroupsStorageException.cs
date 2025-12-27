using Temp.Services.Exceptions;

namespace Temp.Services.Organizations.Exceptions;

public class OrganizationGetInnerGroupsStorageException : NotFoundException
{
    public OrganizationGetInnerGroupsStorageException()
        : base("There are no groups inside organization") {
    }
}