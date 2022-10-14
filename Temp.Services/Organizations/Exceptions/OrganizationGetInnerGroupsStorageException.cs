namespace Temp.Services.Organizations.Exceptions;

public class OrganizationGetInnerGroupsStorageException : Exception
{
    public OrganizationGetInnerGroupsStorageException() :
        base("There are no groups inside organization") {

    }
}

