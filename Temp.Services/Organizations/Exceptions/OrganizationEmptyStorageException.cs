namespace Temp.Services.Organizations.Exceptions;

public class OrganizationEmptyStorageException : Exception
{
    public OrganizationEmptyStorageException() :
        base("No Organizations found in storage") {

    }
}

