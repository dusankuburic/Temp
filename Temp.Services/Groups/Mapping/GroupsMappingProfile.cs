using Temp.Domain.Models;
using Temp.Services.Groups.Models.Commands;
using Temp.Services.Groups.Models.Queries;

namespace Temp.Services.Groups.Mapping;

public class GroupsMappingProfile : Profile
{
    public GroupsMappingProfile() {
        CreateMap<CreateGroupRequest, Group>();
        CreateMap<Group, CreateGroupResponse>();
        CreateMap<Group, GetGroupResponse>();
        CreateMap<UpdateGroupRequest, Group>();
        CreateMap<Group, UpdateGroupResponse>();
        CreateMap<ModeratorGroup, GetModeratorGroupsResponse>()
            .ForMember(a => a.Id, o => o.MapFrom(s => s.Group.Id))
            .ForMember(a => a.Name, o => o.MapFrom(s => s.Group.Name));
        CreateMap<Group, GetModeratorFreeGroupsResponse>();

        CreateMap<Team, InnerTeam>();
    }
}