using Temp.Domain.Models;
using Temp.Services.Workplaces.Exceptions;

namespace Temp.Services.Workplaces;

public partial class WorkplaceService
{
    private void ValidateWorkplaceOnCreate(Workplace workplace) {
        ValidateWorkplace(workplace);
        ValidateWorkplaceStrings(workplace);
    }

    private void ValidateWorkplaceOnUpdate(Workplace workplace) {
        ValidateWorkplace(workplace);
        ValidateWorkplaceStrings(workplace);
    }

    private void ValidateWorkplaceOnStatusUpdate(Workplace workplace) {
        ValidateWorkplace(workplace);
    }

    private void ValidateWorkplace(Workplace workplace) {
        if (workplace is null)
            throw new NullWorkplaceException();
    }

    private void ValidateWorkplaceStrings(Workplace workplace) {
        switch (workplace) {
            case { } when IsInvalid(workplace.Name):
                throw new InvalidWorkplaceException(
                    parameterName: nameof(workplace.Name),
                    parameterValue: workplace.Name);
        }
    }

    private static bool IsInvalid(string input)
        => string.IsNullOrWhiteSpace(input);
}
