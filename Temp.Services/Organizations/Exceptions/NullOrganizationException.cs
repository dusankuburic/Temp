namespace Temp.Services.Organizations.Exceptions;

public class NullOrganizationException : Exception
{
    public NullOrganizationException() : base("Organization is null") { }
}

