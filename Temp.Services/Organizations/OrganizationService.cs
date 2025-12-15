using Temp.Database.UnitOfWork;
using Temp.Domain.Models;
using Temp.Services._Helpers;
using Temp.Services.Integrations.Loggings;
using Temp.Services.Organizations.Models.Commands;
using Temp.Services.Organizations.Models.Queries;
using Temp.Services.Providers;

namespace Temp.Services.Organizations;

public partial class OrganizationService : IOrganizationService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ILoggingBroker _loggingBroker;
    private readonly IIdentityProvider _identityProvider;

    public OrganizationService(
        IUnitOfWork unitOfWork,
        IMapper mapper,
        ILoggingBroker loggingBroker,
        IIdentityProvider identityProvider) {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _loggingBroker = loggingBroker;
        _identityProvider = identityProvider;
    }

    public Task<CreateOrganizationResponse> CreateOrganization(CreateOrganizationRequest request) =>
        TryCatch(async () => {
            var organization = _mapper.Map<Organization>(request);

            organization.SetAuditableInfoOnCreate(await _identityProvider.GetCurrentUser());

            ValidateOrganizationOnCreate(organization);

            await _unitOfWork.Organizations.AddAsync(organization);
            await _unitOfWork.SaveChangesAsync();

            return _mapper.Map<CreateOrganizationResponse>(organization);
        });


    public Task<PagedList<GetOrganizationResponse>> GetPagedOrganizations(GetOrganizationsRequest request) =>
        TryCatch(async () => {
            var organizationsQuery = _unitOfWork.Organizations
                .QueryNoTracking()
                .Where(x => x.IsActive);

            if (!string.IsNullOrEmpty(request.Name)) {
                organizationsQuery = organizationsQuery.Where(x => x.Name.Contains(request.Name));
            }

            organizationsQuery = request.WithGroups switch {
                "all" => organizationsQuery,
                "yes" => organizationsQuery.Where(x => x.HasActiveGroup == true),
                "no" => organizationsQuery.Where(x => x.HasActiveGroup == false),
                _ => organizationsQuery
            };

            organizationsQuery = organizationsQuery.OrderBy(x => x.Name);

            return await PagedList<GetOrganizationResponse>.CreateAsync(
                organizationsQuery.ProjectTo<GetOrganizationResponse>(_mapper.ConfigurationProvider),
                request.PageNumber,
                request.PageSize);
        });

    public Task<GetPagedInnerGroupsResponse> GetPagedInnerGroups(GetOrganizationInnerGroupsRequest request) =>
        TryCatch(async () => {
            var innerGroupsQurey = _unitOfWork.Groups
                .QueryNoTracking()
                .Where(x => x.OrganizationId == request.OrganizationId && x.IsActive);

            if (!string.IsNullOrEmpty(request.Name)) {
                innerGroupsQurey = innerGroupsQurey.Where(x => x.Name.Contains(request.Name));
            }

            innerGroupsQurey = request.WithTeams switch {
                "all" => innerGroupsQurey,
                "yes" => innerGroupsQurey.Where(x => x.HasActiveTeam == true),
                "no" => innerGroupsQurey.Where(x => x.HasActiveTeam == false),
                _ => innerGroupsQurey
            };

            innerGroupsQurey = innerGroupsQurey.OrderBy(x => x.Name);

            var pagedGroups = await PagedList<InnerGroup>.CreateAsync(
                innerGroupsQurey.ProjectTo<InnerGroup>(_mapper.ConfigurationProvider),
                request.PageNumber,
                request.PageSize);

            var organizationName = await _unitOfWork.Organizations
                .QueryNoTracking()
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
            var innerGroups = await _unitOfWork.Groups
                .Query()
                .Include(x => x.Teams.Where(x => x.IsActive))
                .Where(x => x.OrganizationId == id && x.IsActive && x.Teams.Count > 0)
                .OrderBy(x => x.Name)
                .ProjectTo<InnerGroup>(_mapper.ConfigurationProvider)
                .ToListAsync();

            return innerGroups;
        });


    public Task<GetOrganizationResponse> GetOrganization(GetOrganizationRequest request) =>
         TryCatch(async () => {
             var organizaiton = await _unitOfWork.Organizations
                .QueryNoTracking()
                .Where(x => x.Id == request.Id && x.IsActive)
                .ProjectTo<GetOrganizationResponse>(_mapper.ConfigurationProvider)
                .FirstOrDefaultAsync();

             ValidateGetOrganizationViewModel(organizaiton);

             return organizaiton;
         });


    public Task<IEnumerable<GetOrganizationResponse>> GetOrganizations() =>
        TryCatch(async () => {
            var organizations = await _unitOfWork.Organizations
                .Query()
                .Include(x => x.Groups.Where(x => x.IsActive))
                .Where(x => x.IsActive && x.HasActiveGroup && x.Groups.Any(x => x.HasActiveTeam == true))
                .OrderBy(x => x.Name)
                .ProjectTo<GetOrganizationResponse>(_mapper.ConfigurationProvider)
                .ToListAsync();

            ValidateStorageOrganizations(organizations);

            return organizations;
        });


    public Task<UpdateOrganizationResponse> UpdateOrganization(UpdateOrganizationRequest request) =>
         TryCatch(async () => {
             var organization = await _unitOfWork.Organizations
                .FirstOrDefaultAsync(x => x.Id == request.Id);

             organization.SetAuditableInfoOnUpdate(await _identityProvider.GetCurrentUser());

             _mapper.Map(request, organization);

             ValidateOrganizationOnUpdate(organization);

             _unitOfWork.Organizations.Update(organization);
             await _unitOfWork.SaveChangesAsync();

             return new UpdateOrganizationResponse();
         });


    public Task<UpdateOrganizationStatusResponse> UpdateOrganizationStatus(UpdateOrganizationStatusRequest request) =>
        TryCatch(async () => {
            var organization = await _unitOfWork.Organizations
                .FirstOrDefaultAsync(x => x.Id == request.Id);

            organization.IsActive = !organization.IsActive;
            organization.SetAuditableInfoOnUpdate(await _identityProvider.GetCurrentUser());

            ValidateOrganizationOnUpdate(organization);

            _unitOfWork.Organizations.Update(organization);
            await _unitOfWork.SaveChangesAsync();

            return new UpdateOrganizationStatusResponse();
        });

    public Task<bool> OrganizationExists(string name) =>
        TryCatch(async () => {
            return await _unitOfWork.Organizations.AnyAsync(x => x.Name == name);
        });

}

