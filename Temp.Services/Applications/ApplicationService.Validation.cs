﻿using Temp.Domain.Models.Applications;
using Temp.Services.Applications.Exceptions;
using Temp.Services.Applications.Models.Queries;

namespace Temp.Services.Applications
{
    public partial class ApplicationService
    {
        private void ValidateApplicationOnCreate(Application application) {
            ValidateApplication(application);
            ValidateApplicationInts(application);
            ValidateApplicationString(application);
            ValidateApplicationDates(application);
        }

        private void ValidateApplication(Application application) {
            if (application is null) {
                throw new NullApplicationException();
            }
        }

        private void ValidateApplicationInts(Application application) {
            switch (application) {
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

        private void ValidateApplicationDates(Application application) {
            switch (application) {
                case { } when IsInvalidDate(application.CreatedAt):
                    throw new InvalidApplicationException(
                        parameterName: nameof(application.CreatedAt),
                        parameterValue: application.CreatedAt);
            }
        }


        private void ValidateApplicationString(Application application) {
            switch (application) {
                case { } when IsInvalid(application.Content):
                    throw new InvalidApplicationException(
                        parameterName: nameof(application.Content),
                        parameterValue: application.Content);
            }
        }

        private void ValidateGetApplication(GetApplicationResponse application) {
            if (application is null) {
                throw new NullApplicationException();
            }
        }

        private void ValidateGetTeamApplicationsViewModel(IEnumerable<GetTeamApplicationsResponse> applicationViewModels) {
            if (applicationViewModels.Count() == 0) {
                throw new ApplicationWithTeamStorageException();
            }
        }

        private void ValidateGetUserApplications(IEnumerable<GetUserApplicationsResponse> applicationViewModels) {
            if (applicationViewModels.Count() == 0) {
                throw new ApplicationWithUserStorageException();
            }
        }

        private static bool IsInvalidInt(int input) {
            if (input > 0 && input <= int.MaxValue) {
                return false;
            }
            return true;
        }

        private static bool IsInvalidDate(DateTime? input) {
            if (input != DateTime.MinValue) {
                return false;
            }
            return true;
        }

        private static bool IsInvalid(string input) => string.IsNullOrWhiteSpace(input);
    }
}
