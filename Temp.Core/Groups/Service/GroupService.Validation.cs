using System;
using System.Collections.Generic;
using System.Linq;
using Temp.Domain.Models;
using Temp.Domain.Models.Groups.Exceptions;

namespace Temp.Core.Groups.Service;

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

    public void ValidateGroup(Group group) {
        if (group is null) {
            throw new NullGroupException();
        }
    }

    public void ValidateGetGroupViewModel(GetGroup.GroupViewModel group) {
        if (group is null) {
            throw new NullGroupException();
        }
    }

    public void ValidateGetModeratorGroupsViewModel
        (IEnumerable<GetModeratorGroups.ModeratorGroupViewModel> moderatorGroupViewModels) {
        if (moderatorGroupViewModels.Count() == 0) {
            throw new ModeratorGroupsEmptyStorageException();
        }
    }
    public void ValidateGetModeratorFreeGroupsViewModel
        (IEnumerable<GetModeratorFreeGroups.ModeratorFreeGroupViewModel> moderatorFreeGroupViewModel) {
        if (moderatorFreeGroupViewModel is null) {
            throw new NullGroupException();
        }
    }

    public void ValidateGetInnerTeamResponse(GetInnerTeams.Response response) {
        if (response is null) {
            throw new NullGroupInnerTeamsException();
        }
    }


    public void ValidateGetInnerTeamsViewModel
        (IEnumerable<GetInnerTeams.InnerTeamViewModel> innerTeamViewModels) {
        if (innerTeamViewModels.Count() == 0) {
            throw new GroupInnerTeamsStorageException();
        }
    }


    public void ValidateGroupString(Group group) {
        switch (group) {
            case { } when IsInvalid(group.Name):
                throw new InvalidGroupException(
                    parameterName: nameof(group.Name),
                    parameterValue: group.Name);
        }
    }


    public static bool IsInvalid(string input) => String.IsNullOrWhiteSpace(input);
}
