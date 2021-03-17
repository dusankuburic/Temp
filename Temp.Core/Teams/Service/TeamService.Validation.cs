using System;
using Temp.Domain.Models;
using Temp.Domain.Models.Teams.Exceptions;

namespace Temp.Core.Teams.Service
{
    public partial class TeamService
    {
        public void ValidateTeamOnCreate(Team team)
        {
            ValidateTeam(team);
            ValidateTeamString(team);
        }

        public void ValidateTeamOnUpdate(Team team)
        {
            ValidateTeam(team);
            ValidateTeamString(team);
        }

        public void ValidateTeam(Team team)
        {
            if(team is null)
            {
                throw new NullTeamException();
            }
        }


        public void ValidateGetTeamViewModel(GetTeam.TeamViewModel team)
        {
            if(team is null)
            {
                throw new NullTeamException();
            }
        }

        public void ValidateTeamString(Team team)
        {
            switch (team)
            {
                case { } when IsInvalid(team.Name):
                    throw new InvalidTeamException(
                        parameterName: nameof(team.Name),
                        parameterValue: team.Name);
            }
        }


        public static bool IsInvalid(string input) => String.IsNullOrWhiteSpace(input);
    }
}
