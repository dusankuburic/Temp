using AutoMapper.QueryableExtensions;
using Temp.Database;
using Temp.Domain.Models;
using Temp.Services.Integrations.Loggings;
using Temp.Services.Organizations.Models.Commands;
using Temp.Services.Organizations.Models.Queries;

namespace Temp.Services.Organizations;

public partial class OrganizationService : IOrganizationService
{
    private readonly ApplicationDbContext _ctx;
    private readonly IMapper _mapper;
    private readonly ILoggingBroker _loggingBroker;

    public OrganizationService(
        ApplicationDbContext ctx,
        IMapper mapper,
        ILoggingBroker loggingBroker) {
        _ctx = ctx;
        _mapper = mapper;
        _loggingBroker = loggingBroker;
    }

    public Task<CreateOrganizationResponse> CreateOrganization(CreateOrganizationRequest request) {
        return TryCatch(async () => {
            var organization = _mapper.Map<Organization>(request);

            ValidateOrganizationOnCreate(organization);

            _ctx.Organizations.Add(organization);
            await _ctx.SaveChangesAsync();

            return _mapper.Map<CreateOrganizationResponse>(organization);
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
             var organizaiton = await _ctx.Organizations
                .Where(x => x.Id == request.Id && x.IsActive)
                .ProjectTo<GetOrganizationResponse>(_mapper.ConfigurationProvider)
                .AsNoTracking()
                .FirstOrDefaultAsync();

             ValidateGetOrganizationViewModel(organizaiton);

             return organizaiton;
         });


    public Task<IEnumerable<GetOrganizationResponse>> GetOrganizations() {
        return TryCatch(async () => {
            var organizations = await _ctx.Organizations
                .Where(x => x.IsActive)
                .ProjectTo<GetOrganizationResponse>(_mapper.ConfigurationProvider)
                .AsNoTracking()
                .ToListAsync();

            ValidateStorageOrganizations(organizations);

            return organizations;
        });
    }

    public Task<UpdateOrganizationResponse> UpdateOrganization(UpdateOrganizationRequest request) {
        return TryCatch(async () => {
            var organization = await _ctx.Organizations
                .Where(x => x.Id == request.Id)
                .FirstOrDefaultAsync();

            _mapper.Map(request, organization);

            ValidateOrganizationOnUpdate(organization);

            await _ctx.SaveChangesAsync();

            return new UpdateOrganizationResponse();
        });
    }

    public async Task<UpdateOrganizationStatusResponse> UpdateOrganizationStatus(UpdateOrganizationStatusRequest request) {
        var ortanization = await _ctx.Organizations
            .Where(x => x.Id == request.Id)
            .FirstOrDefaultAsync();

        ortanization.IsActive = !ortanization.IsActive;

        await _ctx.SaveChangesAsync();

        return new UpdateOrganizationStatusResponse();
    }

    private async Task<bool> OrganizationExists(string name) {
        return await _ctx.Organizations.AnyAsync(x => x.Name == name);
    }
}

