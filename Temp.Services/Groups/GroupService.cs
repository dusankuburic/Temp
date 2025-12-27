using Temp.Database.UnitOfWork;
using Temp.Domain.Models;
using Temp.Services._Helpers;
using Temp.Services._Shared;
using Temp.Services.Groups.Models.Commands;
using Temp.Services.Groups.Models.Queries;
using Temp.Services.Integrations.Loggings;
using Temp.Services.Providers;

namespace Temp.Services.Groups;

public partial class GroupService : BaseService<Group>, IGroupService
{
    public GroupService(
        IUnitOfWork unitOfWork,
        IMapper mapper,
        ILoggingBroker loggingBroker,
        IIdentityProvider identityProvider)
        : base(unitOfWork, mapper, loggingBroker, identityProvider) {
    }

    public Task<CreateGroupResponse> CreateGroup(CreateGroupRequest request) =>
        TryCatch(async () => {
            var group = Mapper.Map<Group>(request);

            group.SetAuditableInfoOnCreate(await IdentityProvider.GetCurrentUser());

            ValidateGroupOnCreate(group);

            await UnitOfWork.Groups.AddAsync(group);

            var organization = await UnitOfWork.Organizations
                .FirstOrDefaultAsync(x => x.Id == request.OrganizationId);

            organization.HasActiveGroup = true;
            UnitOfWork.Organizations.Update(organization);


            await UnitOfWork.SaveChangesAsync();

            return Mapper.Map<CreateGroupResponse>(group);
        });

    public Task<GetGroupResponse> GetGroup(GetGroupRequest request) =>
        TryCatch(async () => {
            var group = await UnitOfWork.Groups
                .QueryNoTracking()
                .Where(x => x.Id == request.Id && x.IsActive)
                .ProjectTo<GetGroupResponse>(Mapper.ConfigurationProvider)
                .FirstOrDefaultAsync();

            return group;
        });

    public Task<UpdateGroupResponse> UpdateGroup(UpdateGroupRequest request) =>
        TryCatch(async () => {
            var group = await UnitOfWork.Groups
                .FirstOrDefaultAsync(x => x.Id == request.Id);

            Mapper.Map(request, group);

            group.SetAuditableInfoOnUpdate(await IdentityProvider.GetCurrentUser());

            ValidateGroupOnUpdate(group);

            UnitOfWork.Groups.Update(group);
            await UnitOfWork.SaveChangesAsync();

            return Mapper.Map<UpdateGroupResponse>(group);
        });

    public Task<UpdateGroupStatusResponse> UpdateGroupStatus(UpdateGroupStatusRequest request) =>
        TryCatch(async () => {
            var group = await UnitOfWork.Groups
                .Query()
                .Include(x => x.Organization)
                .Where(x => x.Id == request.Id)
                .FirstOrDefaultAsync();

            group.IsActive = !group.IsActive;
            group.SetAuditableInfoOnUpdate(await IdentityProvider.GetCurrentUser());

            ValidateGroupOnStatusUpdate(group);

            UnitOfWork.Groups.Update(group);


            group.Organization.HasActiveGroup = await UnitOfWork.Groups
                .QueryNoTracking()
                .AnyAsync(x => x.OrganizationId == group.OrganizationId && x.IsActive && x.HasActiveTeam);

            UnitOfWork.Organizations.Update(group.Organization);


            await UnitOfWork.SaveChangesAsync();

            return new UpdateGroupStatusResponse();
        });

    public Task<GetPagedGroupInnerTeamsResponse> GetPagedGroupInnerTeams(GetPagedGroupInnerTeamsRequest request) =>
        TryCatch(async () => {
            var innerTeamsQuery = UnitOfWork.Teams
                .QueryNoTracking()
                .Where(x => x.GroupId == request.GroupId && x.IsActive)
                .OrderBy(x => x.Name)
                .ProjectTo<InnerTeam>(Mapper.ConfigurationProvider);

            if (!string.IsNullOrEmpty(request.Name)) {
                innerTeamsQuery = innerTeamsQuery.Where(x => x.Name.Contains(request.Name));
            }

            var pagedTeams = await PagedList<InnerTeam>.CreateAsync(
                innerTeamsQuery,
                request.PageNumber,
                request.PageSize);

            var teamName = await UnitOfWork.Groups
                .QueryNoTracking()
                .Where(x => x.Id == request.GroupId && x.IsActive)
                .Select(x => x.Name)
                .FirstOrDefaultAsync();

            return new GetPagedGroupInnerTeamsResponse {
                Id = request.GroupId,
                Name = teamName,
                Teams = pagedTeams
            };
        });

    public Task<List<InnerTeam>> GetGroupInnerTeams(int id) =>
        TryCatch(async () => {
            var innerTeams = await UnitOfWork.Teams
                .QueryNoTracking()
                .Where(x => x.GroupId == id && x.IsActive)
                .OrderBy(x => x.Name)
                .ProjectTo<InnerTeam>(Mapper.ConfigurationProvider)
                .ToListAsync();

            return innerTeams;
        });

    public Task<List<GetModeratorGroupsResponse>> GetModeratorGroups(GetModeratorGroupsRequest request) =>
        TryCatch(async () => {

            var moderatorGroups = await UnitOfWork.ModeratorGroups
                .QueryNoTracking()
                .Where(x => x.ModeratorId == request.Id)
                .ProjectTo<GetModeratorGroupsResponse>(Mapper.ConfigurationProvider)
                .ToListAsync();

            return moderatorGroups;
        });

    public Task<List<GetModeratorFreeGroupsResponse>> GetModeratorFreeGroups(GetModeratorFreeGroupsRequest request) =>
        TryCatch(async () => {
            var moderatorGroupIds = await UnitOfWork.ModeratorGroups
                .QueryNoTracking()
                .Where(x => x.ModeratorId == request.ModeratorId)
                .Select(x => x.GroupId)
                .ToListAsync();

            var moderatorFreeGroups = await UnitOfWork.Groups
                .QueryNoTracking()
                .Where(x => x.OrganizationId == request.OrganizationId && x.IsActive)
                .Where(x => !moderatorGroupIds.Contains(x.Id))
                .ProjectTo<GetModeratorFreeGroupsResponse>(Mapper.ConfigurationProvider)
                .ToListAsync();

            return moderatorFreeGroups;
        });


    public Task<bool> GroupExists(string name, int organizationId) =>
        TryCatch(async () => {
            return await UnitOfWork.Groups.AnyAsync(x => x.Name == name && x.OrganizationId == organizationId);
        });

}