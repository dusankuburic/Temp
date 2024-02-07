using Temp.Domain.Models;
using Temp.Services.Workplaces.Exceptions;

namespace Temp.Services.Workplaces;

public partial class WorkplaceService
{
    public void ValidateWorkplaceOnCreate(Workplace workplace) {
        ValidateWorkplace(workplace);
        ValidateWorkplaceStrings(workplace);
    }

    public void ValidateWorkplaceOnUpdate(Workplace workplace) {
        ValidateWorkplace(workplace);
        ValidateWorkplaceStrings(workplace);
    }

    public void ValidateWorkplaceOnStatusUpdate(Workplace workplace) {
        ValidateWorkplace(workplace);
    }

    public void ValidateWorkplace(Workplace workplace) {
        if (workplace is null)
            throw new NullWorkplaceException();
    }

    public void ValidateWorkplaceStrings(Workplace workplace) {
        switch (workplace) {
            case { } when IsInvalid(workplace.Name):
                throw new InvalidWorkplaceException(
                    parameterName: nameof(workplace.Name),
                    parameterValue: workplace.Name);
        }
    }

    public static bool IsInvalid(string input)
        => string.IsNullOrWhiteSpace(input);
}
