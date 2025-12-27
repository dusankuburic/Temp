using Temp.Services.Groups.Models.Commands;
using Temp.Services.Groups.Models.Queries;

namespace Temp.Services.Groups;

public interface IGroupService
{
    Task<CreateGroupResponse> CreateGroup(CreateGroupRequest request);
    Task<GetGroupResponse> GetGroup(GetGroupRequest request);
    Task<UpdateGroupResponse> UpdateGroup(UpdateGroupRequest request);
    Task<UpdateGroupStatusResponse> UpdateGroupStatus(UpdateGroupStatusRequest request);
    Task<List<InnerTeam>> GetGroupInnerTeams(int id);
    Task<GetPagedGroupInnerTeamsResponse> GetPagedGroupInnerTeams(GetPagedGroupInnerTeamsRequest request);
    Task<List<GetModeratorGroupsResponse>> GetModeratorGroups(GetModeratorGroupsRequest request);
    Task<List<GetModeratorFreeGroupsResponse>> GetModeratorFreeGroups(GetModeratorFreeGroupsRequest request);
    Task<bool> GroupExists(string name, int organizationId);
}