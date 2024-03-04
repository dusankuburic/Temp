using AutoMapper.QueryableExtensions;
using Temp.Database;
using Temp.Domain.Models;
using Temp.Services._Helpers;
using Temp.Services.Groups.Models.Commands;
using Temp.Services.Groups.Models.Queries;
using Temp.Services.Integrations.Loggings;

namespace Temp.Services.Groups;

public partial class GroupService : IGroupService
{
    private readonly ApplicationDbContext _ctx;
    private readonly IMapper _mapper;
    private readonly ILoggingBroker _loggingBroker;

    public GroupService(
        ApplicationDbContext ctx,
        IMapper mapper,
        ILoggingBroker loggingBroker) {
        _ctx = ctx;
        _mapper = mapper;
        _loggingBroker = loggingBroker;
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

    public Task<GetPagedGroupInnerTeamsResponse> GetPagedGroupInnerTeams(GetPagedGroupInnerTeamsRequest request) =>
        TryCatch(async () => {
            var innerTeamsQuery = _ctx.Teams
                .Where(x => x.GroupId == request.GroupId && x.IsActive)
                .ProjectTo<InnerTeam>(_mapper.ConfigurationProvider)
                .AsQueryable();

            if (!string.IsNullOrEmpty(request.Name)) {
                innerTeamsQuery = innerTeamsQuery.Where(x => x.Name.Contains(request.Name))
                    .AsQueryable();
            }

            var pagedTeams = await PagedList<InnerTeam>.CreateAsync(
                innerTeamsQuery,
                request.PageNumber,
                request.PageSize);

            var teamName = await _ctx.Groups
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
            var innerTeams = await _ctx.Teams
                .Where(x => x.GroupId == id && x.IsActive)
                .ProjectTo<InnerTeam>(_mapper.ConfigurationProvider)
                .ToListAsync();

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


    public Task<bool> GroupExists(string name, int organizationId) =>
        TryCatch(async () => {
            return await _ctx.Groups.AnyAsync(x => x.Name == name && x.OrganizationId == organizationId);
        });

}
