using AutoMapper.QueryableExtensions;
using Temp.Database;
using Temp.Domain.Models;
using Temp.Services.Groups.Models.Commands;
using Temp.Services.Groups.Models.Queries;

namespace Temp.Services.Groups;

public partial class GroupService : IGroupService
{
    private readonly ApplicationDbContext _ctx;
    private readonly IMapper _mapper;

    public GroupService(ApplicationDbContext ctx, IMapper mapper) {
        _ctx = ctx;
        _mapper = mapper;
    }

    public Task<CreateGroupResponse> CreateGroup(CreateGroupRequest request) =>
        TryCatch(async () => {
            var group = _mapper.Map<Group>(request);

            ValidateGroupOnCreate(group);

            _ctx.Groups.Add(group);
            await _ctx.SaveChangesAsync();

            return _mapper.Map<CreateGroupResponse>(group);
        });

    public Task<GetGroupResponse> GetGroup(GetGroupRequest request) =>
        TryCatch(async () => {
            var group = await _ctx.Groups
                .Where(x => x.Id == request.Id && x.IsActive)
                .ProjectTo<GetGroupResponse>(_mapper.ConfigurationProvider)
                .FirstOrDefaultAsync();

            return group;
        });

    public Task<UpdateGroupResponse> UpdateGroup(UpdateGroupRequest request) =>
        TryCatch(async () => {
            var group = await _ctx.Groups
                .Where(x => x.Id == request.Id)
                .FirstOrDefaultAsync();

            _mapper.Map(request, group);

            ValidateGroupOnUpdate(group);

            await _ctx.SaveChangesAsync();

            return _mapper.Map<UpdateGroupResponse>(group);
        });

    public Task<UpdateGroupStatusResponse> UpdateGroupStatus(UpdateGroupStatusRequest request) =>
        TryCatch(async () => {
            var group = await _ctx.Groups
                .Where(x => x.Id == request.Id)
                .FirstOrDefaultAsync();

            ValidateGroupOnStatusUpdate(group);

            group.IsActive = !group.IsActive;

            await _ctx.SaveChangesAsync();

            return new UpdateGroupStatusResponse();
        });

    public Task<GetGroupInnerTeamsResponse> GetGroupInnerTeams(GetGroupInnerTeamsRequest request) =>
        TryCatch(async () => {
            var innerTeams = await _ctx.Groups
                .Include(x => x.Teams)
                .Where(x => x.Id == request.Id && x.IsActive)
                .Select(x => new GetGroupInnerTeamsResponse {
                    Id = x.Id,
                    Name = x.Name,
                    Teams = x.Teams.Select(t => new InnerTeam{ Id = t.Id, Name = t.Name })
                })
                .FirstOrDefaultAsync();

            return innerTeams;
        });

    public Task<List<GetModeratorGroupsResponse>> GetModeratorGroups(GetModeratorGroupsRequest request) =>
        TryCatch(async () => {
            var moderatorGroups = await _ctx.ModeratorGroups
                .Include(x => x.Group)
                .Where(x => x.ModeratorId == request.Id)
                .ProjectTo<GetModeratorGroupsResponse>(_mapper.ConfigurationProvider)
                .AsNoTracking()
                .ToListAsync();

            return moderatorGroups;
        });

    public Task<List<GetModeratorFreeGroupsResponse>> GetModeratorFreeGroups(GetModeratorFreeGroupsRequest request) =>
        TryCatch(async () => {
            var moderatorGroupIds = await _ctx.ModeratorGroups
                .Where(x => x.ModeratorId == request.ModeratorId)
                .Select(x => x.GroupId)
                .ToListAsync();

            var moderatorFreeGroups = await _ctx.Groups
                .Where(x => x.OrganizationId == request.OrganizationId && x.IsActive)
                .Where(x => !moderatorGroupIds.Contains(x.Id))
                .ProjectTo<GetModeratorFreeGroupsResponse>(_mapper.ConfigurationProvider)
                .AsNoTracking()
                .ToListAsync();

            return moderatorFreeGroups;
        });


    private async Task<bool> GroupExists(string name, int organizationId) {
        return await _ctx.Groups
            .AnyAsync(x => x.Name == name && x.OrganizationId == organizationId);
    }
}
