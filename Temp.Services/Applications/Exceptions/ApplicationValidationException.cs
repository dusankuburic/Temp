namespace Temp.Services.Applications.Exceptions
{
    public class ApplicationValidationException : Exception
    {
        public ApplicationValidationException(Exception innerException)
            : base("Invalid input, contact support.", innerException) {

        }
    }
}
