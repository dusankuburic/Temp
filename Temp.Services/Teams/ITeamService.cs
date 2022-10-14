using Temp.Services.Teams.CLI.Command;
using Temp.Services.Teams.CLI.Query;

namespace Temp.Services.Teams;

public interface ITeamService
{
    Task<CreateTeam.Response> CreateTeam(CreateTeam.Request request);
    Task<GetFullTeamTree.TeamTreeViewModel> GetFullTeamTree(int id);
    Task<GetTeam.TeamViewModel> GetTeam(int id);
    Task<UpdateTeam.Response> UpdateTeam(int id, UpdateTeam.Request request);
    Task<bool> UpdateTeamStatus(int teamId);
    Task<GetUserTeam.TeamViewModel> GetUserTeam(int id);
}

