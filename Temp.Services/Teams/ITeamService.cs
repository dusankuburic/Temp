using Temp.Services.Teams.Models.Commands;
using Temp.Services.Teams.Models.Queries;

namespace Temp.Services.Teams;

public interface ITeamService
{
    Task<CreateTeamResponse> CreateTeam(CreateTeamRequest request);
    Task<GetFullTeamTreeResponse> GetFullTeamTree(GetFullTeamTreeRequest request);
    Task<GetTeamResponse> GetTeam(GetTeamRequest request);
    Task<UpdateTeamResponse> UpdateTeam(UpdateTeamRequest request);
    Task<UpdateTeamStatusResponse> UpdateTeamStatus(UpdateTeamStatusRequest request);
    Task<GetUserTeamResponse> GetUserTeam(GetUserTeamRequest request);
    Task<bool> TeamExists(string name, int groupId);
    Task DeleteTeamAsync(int id);
    Task<IEnumerable<GetTeamResponse>> GetAllTeamsAsync();
}