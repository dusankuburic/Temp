using System;
using Temp.Domain.Models;
using Temp.Domain.Models.Applications.Exceptions;

namespace Temp.Core.Applications.Service
{
    public partial class ApplicationService
    {
        
        public void ValidateApplicationOnCreate(Application application)
        {
            ValidateApplication(application);
            ValidateApplicationInts(application);
            ValidateApplicationDates(application);
        }

        public void ValidateApplication(Application application)
        {
            if(application is null)
            {
                throw new NullApplicationException();
            }
        }

        public void ValidateApplicationInts(Application application)
        {
            switch (application)
            {
                case { } when IsInvalidInt(application.UserId):
                    throw new InvalidApplicationException(
                        parameterName: nameof(application.UserId),
                        parameterValue: application.UserId);
                case { } when IsInvalidInt(application.TeamId):
                    throw new InvalidApplicationException(
                        parameterName: nameof(application.TeamId),
                        parameterValue: application.TeamId);
            }          
        }

        public void ValidateApplicationDates(Application application)
        {
            switch (application)
            {
                case { } when IsInvalidDate(application.CreatedAt):
                    throw new InvalidApplicationException(
                        parameterName: nameof(application.CreatedAt),
                        parameterValue: application.CreatedAt);
            }
        }

        public static bool IsInvalidInt(int input)
        {
            if(input > 0 && input <= int.MaxValue)
            {
                return false;
            }
            return true;
        }

        public static bool IsInvalidDate(DateTime input)
        {
            if(input != DateTime.MinValue)
            {
                return false;
            }
            return true;
        }
    }
}
