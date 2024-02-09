using Temp.Domain.Models;
using Temp.Services.Groups.Exceptions;

namespace Temp.Services.Groups;

public partial class GroupService
{
    public void ValidateGroupOnCreate(Group group) {
        ValidateGroup(group);
        ValidateGroupString(group);
    }

    public void ValidateGroupOnUpdate(Group group) {
        ValidateGroup(group);
        ValidateGroupString(group);
    }

    public void ValidateGroupOnStatusUpdate(Group group) {
        ValidateGroup(group);
    }

    public void ValidateGroup(Group group) {
        if (group is null)
            throw new NullGroupException();
    }

    public void ValidateGroupString(Group group) {
        switch (group) {
            case { } when IsInvalid(group.Name):
                throw new InvalidGroupException(
                    parameterName: nameof(group.Name),
                    parameterValue: group.Name);
        }
    }

    public static bool IsInvalid(string input) =>
        string.IsNullOrWhiteSpace(input);
}
