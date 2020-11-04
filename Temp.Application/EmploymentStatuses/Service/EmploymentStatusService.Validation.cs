using System;
using System.Collections.Generic;
using System.Linq;
using Temp.Domain.Models;
using Temp.Domain.Models.EmploymentStatuses.Exceptions;

namespace Temp.Application.EmploymentStatuses.Service
{
    public partial class EmploymentStatusService
    {
        public void ValidateEmploymentStatusOnCreate(EmploymentStatus employmentStatus)
        {
            ValidateEmploymentStatus(employmentStatus);
            ValidateEmploymentStatusStrings(employmentStatus);
        }


        public void ValidateEmploymentStatus(EmploymentStatus employmentStatus)
        {
            if(employmentStatus is null)
            {
                throw new NullEmploymentStatusException();
            }
        }

        public void ValidateEmploymentStatusStrings(EmploymentStatus employmentStatus)
        {
            switch(employmentStatus)
            {
                case {} when IsInvalid(employmentStatus.Name):
                    throw new InvalidEmploymentStatusException(
                        parameterName: nameof(employmentStatus.Name),
                        parameterValue: employmentStatus.Name);
            }
        }

        public void ValidateEmployeeStatuses(IEnumerable<GetEmploymentStatuses.EmploymentStatusViewModel> storageEmployeeStatuses)
        {
            if(storageEmployeeStatuses.Count() == 0)
            {
                throw new EmploymentStatusEmptyStorageException();
            }
        }

        public static bool IsInvalid(string input) => String.IsNullOrWhiteSpace(input);
    }
}
