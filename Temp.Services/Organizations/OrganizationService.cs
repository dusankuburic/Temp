using AutoMapper.QueryableExtensions;
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
                .ProjectTo<GetOrganizationResponse>(_mapper.ConfigurationProvider)
                .AsQueryable();

            if (!string.IsNullOrEmpty(request.Name)) {
                organizationsQuery = organizationsQuery.Where(x => x.Name.Contains(request.Name))
                    .AsQueryable();
            }

            return await PagedList<GetOrganizationResponse>.CreateAsync(
                organizationsQuery,
                request.PageNumber,
                request.PageSize);
        });

    public Task<GetPagedInnerGroupsResponse> GetPagedInnerGroups(GetOrganizationInnerGroupsRequest request) =>
        TryCatch(async () => {
            var innerGroupsQurey = _ctx.Groups
                .Where(x => x.OrganizationId == request.OrganizationId && x.IsActive)
                .ProjectTo<InnerGroup>(_mapper.ConfigurationProvider)
                .AsQueryable();

            if (!string.IsNullOrEmpty(request.Name)) {
                innerGroupsQurey = innerGroupsQurey.Where(x => x.Name.Contains(request.Name))
                    .AsQueryable();
            }

            var pagedGroups = await PagedList<InnerGroup>.CreateAsync(
                innerGroupsQurey,
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

    public Task<GetInnerGroupsResponse> GetInnerGroups(int id) =>
        TryCatch(async () => {
            var innerGroups = await _ctx.Organizations
                .AsNoTracking()
                .Include(x => x.Groups)
                .Where(x => x.Id == id && x.IsActive)
                .ProjectTo<GetInnerGroupsResponse>(_mapper.ConfigurationProvider)
                .FirstOrDefaultAsync();

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
                .Where(x => x.IsActive)
                .ProjectTo<GetOrganizationResponse>(_mapper.ConfigurationProvider)
                .AsNoTracking()
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

