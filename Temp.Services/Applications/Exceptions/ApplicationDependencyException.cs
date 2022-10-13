namespace Temp.Services.Applications.Exceptions
{
    public class ApplicationDependencyException : Exception
    {
        public ApplicationDependencyException(Exception innerException)
            : base("Service dependency error occured, contect support", innerException) { }
    }

}
