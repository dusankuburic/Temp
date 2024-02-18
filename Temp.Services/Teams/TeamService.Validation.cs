using Temp.Domain.Models;
using Temp.Services.Teams.Exceptions;
using Temp.Services.Teams.Models.Queries;

namespace Temp.Services.Teams;

public partial class TeamService
{
    public void ValidateTeamOnCreate(Team team) {
        ValidateTeam(team);
        ValidateTeamString(team);
    }

    public void ValidateTeamOnUpdate(Team team) {
        ValidateTeam(team);
        ValidateTeamString(team);
    }

    public void ValidateTeam(Team team) {
        if (team is null) {
            throw new NullTeamException();
        }
    }


    public void ValidateGetTeam(GetTeamResponse team) {
        if (team is null) {
            throw new NullTeamException();
        }
    }

    public void ValidateGetUserTeam(GetUserTeamResponse team) {
        if (team is null) {
            throw new NullTeamException();
        }
    }

    public void ValidateTeamString(Team team) {
        switch (team) {
            case { } when IsInvalid(team.Name):
                throw new InvalidTeamException(
                    parameterName: nameof(team.Name),
                    parameterValue: team.Name);
        }
    }


    public static bool IsInvalid(string input) {
        return string.IsNullOrWhiteSpace(input);
    }
}

