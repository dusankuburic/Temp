using Temp.Database;
using Temp.Domain.Models;
using Temp.Services.Groups.Models.Command;
using Temp.Services.Groups.Models.Query;

namespace Temp.Services.Groups;

public partial class GroupService : IGroupService
{
    private readonly ApplicationDbContext _ctx;

    public GroupService(ApplicationDbContext ctx) {
        _ctx = ctx;
    }

    public Task<CreateGroupResponse> CreateGroup(CreateGroupRequest request) =>
        TryCatch(async () => {

            //TODO: group exists?

            var group = new Group {
                Name = request.Name,
                OrganizationId = request.OrganizationId
            };

            ValidateGroupOnCreate(group);

            _ctx.Groups.Add(group);
            await _ctx.SaveChangesAsync();


            return new CreateGroupResponse {
                Id = group.Id,
                Name = group.Name,
                OrganizationId = group.OrganizationId
            };
        });

    public Task<GetGroupResponse> GetGroup(GetGroupRequest request) =>
        TryCatch(async () => {
            var group = await _ctx.Groups
                .Where(x => x.Id == request.Id && x.IsActive)
                .Select(x => new GetGroupResponse {
                    Id = x.Id,
                    Name = x.Name,
                    OrganizationId = x.OrganizationId
                })
                .FirstOrDefaultAsync();

            return group;
        });

    public Task<UpdateGroupResponse> UpdateGroup(UpdateGroupRequest request) =>
        TryCatch(async () => {
            var groupExists = await GroupExists(request.Name, request.OrganizationId);
            if (groupExists)
                return new UpdateGroupResponse { Success = false };

            var group = _ctx.Groups.FirstOrDefault(x => x.Id == request.Id);

            group.Name = request.Name;
            ValidateGroupOnUpdate(group);

            await _ctx.SaveChangesAsync();

            return new UpdateGroupResponse {
                Id = group.Id,
                Name = group.Name,
                Success = true
            };
        });

    public Task<UpdateGroupResponse> UpdateGroupStatus(UpdateGroupStatusRequest request) =>
        TryCatch(async () => {
            var group = await _ctx.Groups
                .Where(x => x.Id == request.Id)
                .FirstOrDefaultAsync();

            ValidateGroupOnStatusUpdate(group);

            group.IsActive = !group.IsActive;

            await _ctx.SaveChangesAsync();

            return new UpdateGroupResponse { Success = true };
        });

    public Task<GetGroupInnerTeamsResponse> GetGroupInnerTeams(GetGroupInnerTeamsRequest request) =>
        TryCatch(async () => {
            var innerTeams = await _ctx.Groups
                .Include(x => x.Teams)
                .Where(x => x.Id == request.Id && x.IsActive)
                .Select(x => new GetGroupInnerTeamsResponse {
                    Id = x.Id,
                    Name = x.Name,
                    Teams = x.Teams.Select(t => new InnerTeam{ Id = t.Id, Name = t.Name})
                })
                .FirstOrDefaultAsync();

            return innerTeams;
        });

    public Task<List<GetModeratorGroupsResponse>> GetModeratorGroups(GetModeratorGroupsRequest request) =>
        TryCatch(async () => {
            var moderatorGroups = await _ctx.ModeratorGroups
                .Where(x => x.ModeratorId == request.Id)
                .Include(x => x.Group)
                .Select(x => new GetModeratorGroupsResponse {
                    Id = x.Group.Id,
                    Name = x.Group.Name
                })
                .ToListAsync();

            return moderatorGroups;
        });

    public Task<List<GetModeratorFreeGroupsResponse>> GetModeratorFreeGroups(GetModeratorFreeGroupsRequest request) =>
        TryCatch(async () => {
            var moderatorGroupIds = await _ctx.ModeratorGroups
                .Where(x => x.ModeratorId == request.moderatorId)
                .Select(x => x.GroupId)
                .ToListAsync();

            var moderatorFreeGroups = await _ctx.Groups
                .Where(x => x.OrganizationId == request.organizationId && x.IsActive)
                .Where(x => !moderatorGroupIds.Contains(x.Id))
                .Select(x => new GetModeratorFreeGroupsResponse {
                    Id = x.Id,
                    Name = x.Name
                })
                .ToListAsync();

            return moderatorFreeGroups;
        });


    private async Task<bool> GroupExists(string name, int organizationId) {
        return await _ctx.Groups
            .AnyAsync(x => x.Name == name && x.OrganizationId == organizationId);
    }
}
