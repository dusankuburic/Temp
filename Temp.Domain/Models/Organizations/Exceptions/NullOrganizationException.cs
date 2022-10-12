namespace Temp.Domain.Models.Organizations.Exceptions;

public class NullOrganizationException : Exception
{
    public NullOrganizationException() : base("Organization is null") { }
}
