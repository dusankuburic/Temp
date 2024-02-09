namespace Temp.Services.Workplaces.Exceptions;

public class NullWorkplaceException : Exception
{
    public NullWorkplaceException() : base("Workplace is null") { }
}
