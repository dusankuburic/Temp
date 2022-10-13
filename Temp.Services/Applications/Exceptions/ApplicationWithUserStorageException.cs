namespace Temp.Services.Applications.Exceptions
{
    public class ApplicationWithUserStorageException : Exception
    {
        public ApplicationWithUserStorageException() :
            base("No applications found in storage for chosen user") {

        }
    }
}
