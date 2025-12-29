using Temp.Database.UnitOfWork;
using Temp.Domain.Models;
using Temp.Services._Helpers;
using Temp.Services._Shared;
using Temp.Services.Integrations.Azure.AzureStorage;
using Temp.Services.Integrations.Loggings;
using Temp.Services.Organizations.Models.Commands;
using Temp.Services.Organizations.Models.Queries;
using Temp.Services.Providers;

namespace Temp.Services.Organizations;

public partial class OrganizationService : BaseService<Organization>, IOrganizationService
{
    private readonly IAzureStorageService _azureStorageService;

    public OrganizationService(
        IUnitOfWork unitOfWork,
        IMapper mapper,
        ILoggingBroker loggingBroker,
        IIdentityProvider identityProvider,
        IAzureStorageService azureStorageService)
        : base(unitOfWork, mapper, loggingBroker, identityProvider) {
        _azureStorageService = azureStorageService;
    }

    public Task<CreateOrganizationResponse> CreateOrganization(CreateOrganizationRequest request) =>
        TryCatch(async () => {
            var organization = Mapper.Map<Organization>(request);

            organization.SetAuditableInfoOnCreate(await IdentityProvider.GetCurrentUser());

            ValidateOrganizationOnCreate(organization);

            await UnitOfWork.Organizations.AddAsync(organization);
            await UnitOfWork.SaveChangesAsync();

            return Mapper.Map<CreateOrganizationResponse>(organization);
        });


    public Task<PagedList<GetOrganizationResponse>> GetPagedOrganizations(GetOrganizationsRequest request) =>
        TryCatch(async () => {
            var organizationsQuery = UnitOfWork.Organizations
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
                organizationsQuery.ProjectTo<GetOrganizationResponse>(Mapper.ConfigurationProvider),
                request.PageNumber,
                request.PageSize);
        });

    public Task<GetPagedInnerGroupsResponse> GetPagedInnerGroups(GetOrganizationInnerGroupsRequest request) =>
        TryCatch(async () => {
            var innerGroupsQurey = UnitOfWork.Groups
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
                innerGroupsQurey.ProjectTo<InnerGroup>(Mapper.ConfigurationProvider),
                request.PageNumber,
                request.PageSize);

            var organizationName = await UnitOfWork.Organizations
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

            var innerGroups = await UnitOfWork.Groups
                .QueryNoTracking()
                .Where(x => x.OrganizationId == id && x.IsActive && x.Teams.Any(t => t.IsActive))
                .OrderBy(x => x.Name)
                .ProjectTo<InnerGroup>(Mapper.ConfigurationProvider)
                .ToListAsync();

            return innerGroups;
        });


    public Task<GetOrganizationResponse> GetOrganization(GetOrganizationRequest request) =>
         TryCatch(async () => {
             var organizaiton = await UnitOfWork.Organizations
                .QueryNoTracking()
                .Where(x => x.Id == request.Id && x.IsActive)
                .ProjectTo<GetOrganizationResponse>(Mapper.ConfigurationProvider)
                .FirstOrDefaultAsync();

             ValidateGetOrganizationViewModel(organizaiton);

             return organizaiton;
         });


    public Task<IEnumerable<GetOrganizationResponse>> GetOrganizations() =>
        TryCatch(async () => {

            var organizations = await UnitOfWork.Organizations
                .QueryNoTracking()
                .Where(x => x.IsActive && x.HasActiveGroup && x.Groups.Any(g => g.IsActive && g.HasActiveTeam))
                .OrderBy(x => x.Name)
                .ProjectTo<GetOrganizationResponse>(Mapper.ConfigurationProvider)
                .ToListAsync();

            ValidateStorageOrganizations(organizations);

            return organizations;
        });


    public Task<UpdateOrganizationResponse> UpdateOrganization(UpdateOrganizationRequest request) =>
         TryCatch(async () => {
             var organization = await UnitOfWork.Organizations
                .FirstOrDefaultAsync(x => x.Id == request.Id);

             string? oldProfilePictureUrl = organization.ProfilePictureUrl;

             organization.SetAuditableInfoOnUpdate(await IdentityProvider.GetCurrentUser());

             Mapper.Map(request, organization);

             ValidateOrganizationOnUpdate(organization);

             UnitOfWork.Organizations.Update(organization);
             await UnitOfWork.SaveChangesAsync();

             if (!string.IsNullOrEmpty(oldProfilePictureUrl) &&
                 oldProfilePictureUrl != request.ProfilePictureUrl) {
                 try {
                     await _azureStorageService.DeleteAsync(oldProfilePictureUrl);
                 } catch (Exception ex) {
                     Logger.LogError(ex);
                 }
             }

             return new UpdateOrganizationResponse();
         });


    public Task<UpdateOrganizationStatusResponse> UpdateOrganizationStatus(UpdateOrganizationStatusRequest request) =>
        TryCatch(async () => {
            var organization = await UnitOfWork.Organizations
                .FirstOrDefaultAsync(x => x.Id == request.Id);

            organization.IsActive = !organization.IsActive;
            organization.SetAuditableInfoOnUpdate(await IdentityProvider.GetCurrentUser());

            ValidateOrganizationOnUpdate(organization);

            UnitOfWork.Organizations.Update(organization);
            await UnitOfWork.SaveChangesAsync();

            return new UpdateOrganizationStatusResponse();
        });

    public Task<bool> OrganizationExists(string name) =>
        TryCatch(async () => {
            return await UnitOfWork.Organizations.AnyAsync(x => x.Name == name);
        });

}