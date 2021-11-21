using System;

namespace Temp.Domain.Models.Applications.Exceptions
{
    public class NullApplicationException : Exception
    {
        public NullApplicationException() :
            base("The application is null") {

        }
    }
}