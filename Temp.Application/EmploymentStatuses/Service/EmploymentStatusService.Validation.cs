using System;
using Temp.Domain.Models;
using Temp.Domain.Models.EmploymentStatuses.Exceptions;
using Temp.Domain.Models.Workplaces.Exceptions;

namespace Temp.Application.EmploymentStatuses.Service
{
    public partial class EmploymentStatusService
    {
        public void ValidateEmploymentStatusOnCreate(EmploymentStatus employmentStatus)
        {

        }


        public void ValidateEmploymentStatus(EmploymentStatus employmentStatus)
        {
            if(employmentStatus is null)
            {
                throw new NullWorkplaceException();
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

        public static bool IsInvalid(string input) => String.IsNullOrWhiteSpace(input);
    }
}
