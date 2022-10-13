namespace Temp.Services.Applications.Exceptions
{
    public class NullApplicationException : Exception
    {
        public NullApplicationException() :
            base("The application is null") {

        }
    }
}
