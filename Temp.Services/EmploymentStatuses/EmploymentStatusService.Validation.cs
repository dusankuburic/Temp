using Temp.Domain.Models;
using Temp.Services.EmploymentStatuses.Exceptions;

namespace Temp.Services.EmploymentStatuses;

public partial class EmploymentStatusService
{
    private void ValidateEmploymentStatusOnCreate(EmploymentStatus employmentStatus) {
        ValidateEmploymentStatus(employmentStatus);
        ValidateEmploymentStatusStrings(employmentStatus);
    }

    private void ValidateEmploymentStatusOnUpdate(EmploymentStatus employmentStatus) {
        ValidateEmploymentStatus(employmentStatus);
        ValidateEmploymentStatusStrings(employmentStatus);
    }

    private void ValidateEmploymentStatus(EmploymentStatus employmentStatus) {
        if (employmentStatus is null) {
            throw new NullEmploymentStatusException();
        }
    }

    private void ValidateEmploymentStatusStrings(EmploymentStatus employmentStatus) {
        switch (employmentStatus) {
            case { } when IsInvalid(employmentStatus.Name):
                throw new InvalidEmploymentStatusException(
                    parameterName: nameof(employmentStatus.Name),
                    parameterValue: employmentStatus.Name);
        }
    }

    private static bool IsInvalid(string input) =>
        string.IsNullOrWhiteSpace(input);
}