﻿using Temp.Domain.Models;
using Temp.Services.Teams.Models.Commands;
using Temp.Services.Teams.Models.Queries;

namespace Temp.Services.Teams.Mappings;

public class TeamsMappingProfile : Profile
{
    public TeamsMappingProfile() {
        CreateMap<CreateTeamRequest, Team>();
        CreateMap<Team, CreateTeamResponse>();
        CreateMap<Team, GetFullTeamTreeResponse>()
            .ForMember(a => a.OrganizationName, o => o.MapFrom(s => s.Group.Organization.Name))
            .ForMember(a => a.OrganizationId, o => o.MapFrom(s => s.Group.Organization.Id))
            .ForMember(a => a.GroupName, o => o.MapFrom(s => s.Group.Name))
            .ForMember(a => a.GroupId, o => o.MapFrom(s => s.Group.Id))
            .ForMember(a => a.TeamName, o => o.MapFrom(s => s.Name))
            .ForMember(a => a.TeamId, o => o.MapFrom(s => s.Id));
        CreateMap<Team, GetTeamResponse>()
            .ForMember(a => a.GroupId, o => o.MapFrom(s => s.Group.Id));
        CreateMap<Employee, GetUserTeamResponse>()
            .ForMember(a => a.Id, o => o.MapFrom(s => s.Team.Id))
            .ForMember(a => a.Name, o => o.MapFrom(s => s.Team.Name));
        CreateMap<UpdateTeamRequest, Team>();

    }
}
