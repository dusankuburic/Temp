using Temp.Database;
using Temp.Domain.Models;
using Temp.Services.Organizations.Models.Commands;
using Temp.Services.Organizations.Models.Queries;

namespace Temp.Services.Organizations;

public partial class OrganizationService : IOrganizationService
{
    private readonly ApplicationDbContext _ctx;
    private readonly IMapper _mapper;

    public OrganizationService(ApplicationDbContext ctx, IMapper mapper) {
        _ctx = ctx;
        _mapper = mapper;
    }

    public Task<CreateOrganizationResponse> CreateOrganization(CreateOrganizationRequest request) {
        return TryCatch(async () => {

            var organization = new Organization
            {
                Name = request.Name,
                IsActive = true
            };

            ValidateOrganizationOnCreate(organization);

            _ctx.Organizations.Add(organization);
            await _ctx.SaveChangesAsync();

            return new CreateOrganizationResponse {
                Id = organization.Id,
                Name = organization.Name
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

    public Task<GetOrganizationResponse> GetOrganization(GetOrganizationRequest request) =>
         TryCatch(async () => {
             var res = await _ctx.Organizations
            .AsNoTracking()
            .Where(x => x.Id == request.Id && x.IsActive)
            .Select(x => new GetOrganizationResponse
            {
                Id = x.Id,
                Name = x.Name
            })
            .FirstOrDefaultAsync();

             ValidateGetOrganizationViewModel(res);
             return res;
         });


    public Task<IEnumerable<GetOrganizationResponse>> GetOrganizations() {
        return TryCatch(async () => {
            var res = await _ctx.Organizations
            .AsNoTracking()
            .Where(x => x.IsActive)
            .Select(x => new GetOrganizationResponse
            {
                Id = x.Id,
                Name = x.Name
            })
            .ToListAsync();

            ValidateStorageOrganizations(res);

            return res;
        });
    }

    public Task<UpdateOrganizationResponse> UpdateOrganization(UpdateOrganizationRequest request) {
        return TryCatch(async () => {
            var organization = _ctx.Organizations
                .FirstOrDefault(x => x.Id == request.Id);

            organization.Name = request.Name;

            ValidateOrganizationOnUpdate(organization);

            await _ctx.SaveChangesAsync();

            return new UpdateOrganizationResponse {
                Success = true
            };
        });
    }

    public async Task<UpdateOrganizationStatusResponse> UpdateOrganizationStatus(UpdateOrganizationStatusRequest request) {
        var ortanization = await _ctx.Organizations
            .Where(x => x.Id == request.Id)
            .FirstOrDefaultAsync();

        ortanization.IsActive = !ortanization.IsActive;

        await _ctx.SaveChangesAsync();

        return new UpdateOrganizationStatusResponse {
            Success = true
        };
    }


    private async Task<bool> OrganizationExists(string name) {
        return await _ctx.Organizations.AnyAsync(x => x.Name == name);
    }
}

