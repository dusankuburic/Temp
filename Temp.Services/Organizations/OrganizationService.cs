using Temp.Database;
using Temp.Domain.Models;
using Temp.Services.Organizations.CLI.Command;
using Temp.Services.Organizations.CLI.Query;

namespace Temp.Services.Organizations;

public partial class OrganizationService : IOrganizationService
{
    private readonly ApplicationDbContext _ctx;
    private readonly IMapper _mapper;

    public OrganizationService(ApplicationDbContext ctx, IMapper mapper) {
        _ctx = ctx;
        _mapper = mapper;
    }

    public Task<CreateOrganization.Response> CreateOrganization(CreateOrganization.Request request) =>
    TryCatch(async () => {
        var organizationExists = await OrganizationExists(request.Name);
        if (organizationExists) {
            return new CreateOrganization.Response {
                Message = $"Error {request.Name} already exists",
                Status = false
            };
        }

        var organization = new Organization
        {
            Name = request.Name
        };

        ValidateOrganizationOnCreate(organization);
        return await new CreateOrganization(_ctx).Do(organization);
    });


    public Task<string> GetInnerGroups(int id) =>
    TryCatch(async () => {
        var res = await new GetInnerGroups(_ctx).Do(id);
        ValidateStorageOrganizationInnerGroups(res.Groups);
        return JsonConvert.SerializeObject(res, Formatting.Indented, new JsonSerializerSettings {
            ContractResolver = new CamelCasePropertyNamesContractResolver()
        });
    });

    public Task<GetOrganization.OrganizationViewModel> GetOrganization(int id) =>
    TryCatch(async () => {
        var res = await new GetOrganization(_ctx).Do(id);
        ValidateGetOrganizationViewModel(res);
        return res;
    });

    public Task<IEnumerable<GetOrganizations.OrganizationViewModel>> GetOrganizations() =>
    TryCatch(async () => {
        var res = await new GetOrganizations(_ctx).Do();
        ValidateStorageOrganizations(res);
        return res;
    });
    public Task<UpdateOrganization.Response> UpdateOrganization(int id, UpdateOrganization.Request request) =>
    TryCatch(async () => {
        var organization = _ctx.Organizations
            .FirstOrDefault(x => x.Id == id);

        if (organization.Name.Equals(request.Name)) {
            return new UpdateOrganization.Response {
                Id = organization.Id,
                Name = organization.Name,
                Message = "Organization name is same",
                Status = true
            };
        }

        var organizationExists = await OrganizationExists(request.Name);

        if (organizationExists) {
            return new UpdateOrganization.Response {
                Id = organization.Id,
                Name = organization.Name,
                Message = $"Organization already exists with {request.Name} name",
                Status = false
            };
        }

        organization.Name = request.Name;

        ValidateOrganizationOnUpdate(organization);

        return await new UpdateOrganization(_ctx).Do(organization);
    });


    public async Task<bool> UpdateOrganizationStatus(int id) =>
        await new UpdateOrganizationStatus(_ctx).Do(id);


    private async Task<bool> OrganizationExists(string name) {
        if (await _ctx.Organizations.AnyAsync(x => x.Name == name))
            return true;

        return false;
    }
}

