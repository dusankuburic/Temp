using Temp.Domain.Models;
using Temp.Services.Groups.Exceptions;

namespace Temp.Services.Groups;

public partial class GroupService
{
    private void ValidateGroupOnCreate(Group group) {
        ValidateGroup(group);
        ValidateGroupString(group);
    }

    private void ValidateGroupOnUpdate(Group group) {
        ValidateGroup(group);
        ValidateGroupString(group);
    }

    private void ValidateGroupOnStatusUpdate(Group group) {
        ValidateGroup(group);
    }

    private void ValidateGroup(Group group) {
        if (group is null)
            throw new NullGroupException();
    }

    private void ValidateGroupString(Group group) {
        switch (group) {
            case { } when IsInvalid(group.Name):
                throw new InvalidGroupException(
                    parameterName: nameof(group.Name),
                    parameterValue: group.Name);
        }
    }

    private static bool IsInvalid(string input) =>
        string.IsNullOrWhiteSpace(input);
}