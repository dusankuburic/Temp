using System;

namespace Temp.Domain.Models.Organizations.Exceptions;

public class OrganizationEmptyStorageException : Exception
{
    public OrganizationEmptyStorageException() :
        base("No Organizations found in storage") {

    }
}
