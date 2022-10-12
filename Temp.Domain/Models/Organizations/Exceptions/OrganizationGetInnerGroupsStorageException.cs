namespace Temp.Domain.Models.Organizations.Exceptions;

public class OrganizationGetInnerGroupsStorageException : Exception
{
    public OrganizationGetInnerGroupsStorageException() :
        base("There are no groups inside organization") {

    }
}
