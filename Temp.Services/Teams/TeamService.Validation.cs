using Temp.Domain.Models;
using Temp.Services.Teams.Exceptions;
using Temp.Services.Teams.Models.Queries;

namespace Temp.Services.Teams;

public partial class TeamService
{
    private void ValidateTeamOnCreate(Team team) {
        ValidateTeam(team);
        ValidateTeamString(team);
    }

    private void ValidateTeamOnUpdate(Team team) {
        ValidateTeam(team);
        ValidateTeamString(team);
    }

    private void ValidateTeam(Team team) {
        if (team is null) {
            throw new NullTeamException();
        }
    }

    private void ValidateGetTeam(GetTeamResponse team) {
        if (team is null) {
            throw new NullTeamException();
        }
    }

    private void ValidateGetUserTeam(GetUserTeamResponse team) {
        if (team is null) {
            throw new NullTeamException();
        }
    }

    private void ValidateTeamString(Team team) {
        switch (team) {
            case { } when IsInvalid(team.Name):
                throw new InvalidTeamException(
                    parameterName: nameof(team.Name),
                    parameterValue: team.Name);
        }
    }

    private static bool IsInvalid(string input) {
        return string.IsNullOrWhiteSpace(input);
    }
}

