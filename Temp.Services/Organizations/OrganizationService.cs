using Temp.Database;
using Temp.Domain.Models;
using Temp.Services._Helpers;
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

    public Task<CreateOrganizationResponse> CreateOrganization(CreateOrganizationRequest request) =>
        TryCatch(async () => {
            var organization = _mapper.Map<Organization>(request);

            ValidateOrganizationOnCreate(organization);

            _ctx.Organizations.Add(organization);
            await _ctx.SaveChangesAsync();

            return _mapper.Map<CreateOrganizationResponse>(organization);
        });


    public Task<PagedList<GetOrganizationResponse>> GetPagedOrganizations(GetOrganizationsRequest request) =>
        TryCatch(async () => {
            var organizationsQuery = _ctx.Organizations
                .Where(x => x.IsActive)
                .AsQueryable();

            if (!string.IsNullOrEmpty(request.Name)) {
                organizationsQuery = organizationsQuery.Where(x => x.Name.Contains(request.Name))
                    .AsQueryable();
            }

            organizationsQuery = request.WithGroups switch {
                "all" => organizationsQuery,
                "yes" => organizationsQuery.Where(x => x.HasActiveGroup == true).AsQueryable(),
                "no" => organizationsQuery.Where(x => x.HasActiveGroup == false).AsQueryable(),
                _ => organizationsQuery
            };

            return await PagedList<GetOrganizationResponse>.CreateAsync(
                organizationsQuery.ProjectTo<GetOrganizationResponse>(_mapper.ConfigurationProvider),
                request.PageNumber,
                request.PageSize);
        });

    public Task<GetPagedInnerGroupsResponse> GetPagedInnerGroups(GetOrganizationInnerGroupsRequest request) =>
        TryCatch(async () => {
            var innerGroupsQurey = _ctx.Groups
                .Where(x => x.OrganizationId == request.OrganizationId && x.IsActive)
                .AsQueryable();

            if (!string.IsNullOrEmpty(request.Name)) {
                innerGroupsQurey = innerGroupsQurey.Where(x => x.Name.Contains(request.Name))
                    .AsQueryable();
            }

            innerGroupsQurey = request.WithTeams switch {
                "all" => innerGroupsQurey,
                "yes" => innerGroupsQurey.Where(x => x.HasActiveTeam == true).AsQueryable(),
                "no" => innerGroupsQurey.Where(x => x.HasActiveTeam == false).AsQueryable(),
                _ => innerGroupsQurey
            };

            var pagedGroups = await PagedList<InnerGroup>.CreateAsync(
                innerGroupsQurey.ProjectTo<InnerGroup>(_mapper.ConfigurationProvider),
                request.PageNumber,
                request.PageSize);

            var organizationName = await _ctx.Organizations
                .Where(x => x.Id == request.OrganizationId && x.IsActive)
                .Select(x => x.Name)
                .FirstOrDefaultAsync();

            return new GetPagedInnerGroupsResponse {
                Id = request.OrganizationId,
                Name = organizationName,
                Groups = pagedGroups
            };
        });

    public Task<List<InnerGroup>> GetInnerGroups(int id) =>
        TryCatch(async () => {
            var innerGroups = await _ctx.Groups
                .Include(x => x.Teams.Where(x => x.IsActive))
                .Where(x => x.OrganizationId == id && x.IsActive && x.Teams.Count > 0)
                .ProjectTo<InnerGroup>(_mapper.ConfigurationProvider)
                .ToListAsync();

            return innerGroups;
        });


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


    public Task<IEnumerable<GetOrganizationResponse>> GetOrganizations() =>
        TryCatch(async () => {
            var organizations = await _ctx.Organizations
                .Include(x => x.Groups.Where(x => x.IsActive))
                .Where(x => x.IsActive && x.HasActiveGroup && x.Groups.Any(x => x.HasActiveTeam == true))
                .ProjectTo<GetOrganizationResponse>(_mapper.ConfigurationProvider)
                .ToListAsync();

            ValidateStorageOrganizations(organizations);

            return organizations;
        });


    public Task<UpdateOrganizationResponse> UpdateOrganization(UpdateOrganizationRequest request) =>
         TryCatch(async () => {
             var organization = await _ctx.Organizations
                .Where(x => x.Id == request.Id)
                .FirstOrDefaultAsync();

             _mapper.Map(request, organization);

             ValidateOrganizationOnUpdate(organization);

             await _ctx.SaveChangesAsync();

             return new UpdateOrganizationResponse();
         });


    public Task<UpdateOrganizationStatusResponse> UpdateOrganizationStatus(UpdateOrganizationStatusRequest request) =>
        TryCatch(async () => {
            var organization = await _ctx.Organizations
            .Where(x => x.Id == request.Id)
            .FirstOrDefaultAsync();

            organization.IsActive = !organization.IsActive;

            ValidateOrganizationOnUpdate(organization);

            await _ctx.SaveChangesAsync();

            return new UpdateOrganizationStatusResponse();
        });

    public Task<bool> OrganizationExists(string name) =>
        TryCatch(async () => {
            return await _ctx.Organizations.AnyAsync(x => x.Name == name);
        });

}

