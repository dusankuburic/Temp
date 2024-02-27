using Temp.Domain.Models;
using Temp.Services.Organizations.Models.Commands;
using Temp.Services.Organizations.Models.Queries;

namespace Temp.Services.Organizations.Mapping;

public class OrganizationsMappingProfile : Profile
{
    public OrganizationsMappingProfile() {
        CreateMap<CreateOrganizationRequest, Organization>();
        CreateMap<Organization, CreateOrganizationResponse>();
        CreateMap<Organization, GetOrganizationResponse>();
        CreateMap<UpdateOrganizationRequest, Organization>();
        ////CreateMap<Organization, GetInnerGroupsResponse>()
        ////    .ForMember(a => a.Groups, o => o.MapFrom(s => s.Groups));

        //CreateMap<Organization, IEnumerable<InnerGroup>>()
        //    .ForMember(a => a, o => o.MapFrom(s => s.Groups));
        CreateMap<Group, InnerGroup>();
    }
}
