﻿namespace Temp.Services.Applications.Exceptions
{
    public class ApplicationServiceException : Exception
    {
        public ApplicationServiceException(Exception innerException)
            : base("Service error, contact support", innerException) {

        }
    }

}
