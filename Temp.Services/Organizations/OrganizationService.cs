using Temp.Database;
using Temp.Domain.Models;
using Temp.Services.Organizations.Models.Command;
using Temp.Services.Organizations.Models.Query;

namespace Temp.Services.Organizations;

public partial class OrganizationService : IOrganizationService
{
    private readonly ApplicationDbContext _ctx;
    private readonly IMapper _mapper;

    public OrganizationService(ApplicationDbContext ctx, IMapper mapper) {
        _ctx = ctx;
        _mapper = mapper;
    }

    public Task<CreateOrganization.Response> CreateOrganization(CreateOrganization.Request request) {
        return TryCatch(async () => {
            var organizationExists = await OrganizationExists(request.Name);

            if (organizationExists) {
                return new CreateOrganization.Response {
                    Message = $"Error {request.Name} already exists",
                    Status = false
                };
            }

            var organization = new Organization
        {
                Name = request.Name,
                IsActive = true
            };

            ValidateOrganizationOnCreate(organization);

            _ctx.Organizations.Add(organization);
            await _ctx.SaveChangesAsync();

            return new CreateOrganization.Response {
                Message = $"Success {organization.Name} is added",
                Status = true
            };
        });
    }

    public Task<string> GetInnerGroups(int id) {
        return TryCatch(async () => {
            var res = await _ctx.Organizations
            .AsNoTracking()
            .Include(x => x.Groups)
            .Where(x => x.Id == id && x.IsActive)
            .Select(x => new GetInnerGroups.Response
            {
                Name = x.Name,
                Groups = x.Groups.Select(g => new GetInnerGroups.InnerGroupViewModel
                {
                    Id = g.Id,
                    Name = g.Name
                })
            })
            .FirstOrDefaultAsync();

            ValidateStorageOrganizationInnerGroups(res.Groups);

            return JsonConvert.SerializeObject(res, Formatting.Indented, new JsonSerializerSettings {
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            });
        });
    }

    public Task<GetOrganization.OrganizationViewModel> GetOrganization(int id) {
        return TryCatch(async () => {
            var res = await _ctx.Organizations
            .AsNoTracking()
            .Where(x => x.Id == id && x.IsActive)
            .Select(x => new GetOrganization.OrganizationViewModel
            {
                Id = x.Id,
                Name = x.Name
            })
            .FirstOrDefaultAsync();

            ValidateGetOrganizationViewModel(res);
            return res;
        });
    }

    public Task<IEnumerable<GetOrganizations.OrganizationViewModel>> GetOrganizations() {
        return TryCatch(async () => {
            var res = await _ctx.Organizations
            .AsNoTracking()
            .Where(x => x.IsActive)
            .Select(x => new GetOrganizations.OrganizationViewModel
            {
                Id = x.Id,
                Name = x.Name
            })
            .ToListAsync();

            ValidateStorageOrganizations(res);

            return res;
        });
    }

    public Task<UpdateOrganization.Response> UpdateOrganization(int id, UpdateOrganization.Request request) {
        return TryCatch(async () => {
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

            await _ctx.SaveChangesAsync();

            return new UpdateOrganization.Response {
                Id = organization.Id,
                Name = organization.Name,
                Message = "Success",
                Status = true
            };
        });
    }

    public async Task<bool> UpdateOrganizationStatus(int id) {
        var ortanization = await _ctx.Organizations
            .Where(x => x.Id == id)
            .FirstOrDefaultAsync();

        ortanization.IsActive = false;

        await _ctx.SaveChangesAsync();

        return true;
    }


    private async Task<bool> OrganizationExists(string name) {
        return await _ctx.Organizations.AnyAsync(x => x.Name == name);
    }
}

