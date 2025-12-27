using Temp.Database.UnitOfWork;
using Temp.Domain.Models;
using Temp.Services._Shared;
using Temp.Services.Integrations.Loggings;
using Temp.Services.Providers;
using Temp.Services.Teams.Exceptions;
using Temp.Services.Teams.Models.Commands;
using Temp.Services.Teams.Models.Queries;

namespace Temp.Services.Teams;

public partial class TeamService : BaseService<Team>, ITeamService
{
    public TeamService(
        IUnitOfWork unitOfWork,
        IMapper mapper,
        ILoggingBroker loggingBroker,
        IIdentityProvider identityProvider)
        : base(unitOfWork, mapper, loggingBroker, identityProvider) {
    }

    public Task<CreateTeamResponse> CreateTeam(CreateTeamRequest request) =>
         TryCatch(async () => {
             var team = Mapper.Map<Team>(request);

             team.SetAuditableInfoOnCreate(await IdentityProvider.GetCurrentUser());

             ValidateTeamOnCreate(team);

             await UnitOfWork.Teams.AddAsync(team);

             var group = await UnitOfWork.Groups
                .Query()
                .Include(x => x.Organization)
                .Where(x => x.Id == request.GroupId)
                .FirstOrDefaultAsync();

             group.HasActiveTeam = true;
             group.Organization.HasActiveGroup = true;

             await UnitOfWork.SaveChangesAsync();

             return Mapper.Map<CreateTeamResponse>(team);
         });

    public Task<GetFullTeamTreeResponse> GetFullTeamTree(GetFullTeamTreeRequest requst) =>
        TryCatch(async () => {
            var team = await UnitOfWork.Teams
                .QueryNoTracking()
                .Where(x => x.Id == requst.Id && x.IsActive)
                .ProjectTo<GetFullTeamTreeResponse>(Mapper.ConfigurationProvider)
                .FirstOrDefaultAsync();

            return team;
        });

    public Task<GetTeamResponse> GetTeam(GetTeamRequest request) =>
       TryCatch(async () => {
           var team = await UnitOfWork.Teams
                .QueryNoTracking()
                .Where(x => x.Id == request.Id && x.IsActive)
                .ProjectTo<GetTeamResponse>(Mapper.ConfigurationProvider)
                .FirstOrDefaultAsync();

           ValidateGetTeam(team);

           return team;
       });

    public Task<GetUserTeamResponse> GetUserTeam(GetUserTeamRequest request) =>
        TryCatch(async () => {
            var team = await UnitOfWork.Employees
                .QueryNoTracking()
                .Where(x => x.Id == request.Id)
                .ProjectTo<GetUserTeamResponse>(Mapper.ConfigurationProvider)
                .FirstOrDefaultAsync();

            ValidateGetUserTeam(team);

            return team;
        });

    public Task<UpdateTeamResponse> UpdateTeam(UpdateTeamRequest request) =>
        TryCatch(async () => {
            var team = await UnitOfWork.Teams
                .FirstOrDefaultAsync(x => x.Id == request.Id);

            Mapper.Map(request, team);

            team.SetAuditableInfoOnUpdate(await IdentityProvider.GetCurrentUser());

            ValidateTeamOnUpdate(team);
            UnitOfWork.Teams.Update(team);
            await UnitOfWork.SaveChangesAsync();

            return new UpdateTeamResponse();
        });

    public Task<UpdateTeamStatusResponse> UpdateTeamStatus(UpdateTeamStatusRequest request) =>
        TryCatch(async () => {
            var team = await UnitOfWork.Teams
                .FirstOrDefaultAsync(x => x.Id == request.Id);

            team.IsActive = !team.IsActive;
            team.SetAuditableInfoOnUpdate(await IdentityProvider.GetCurrentUser());

            ValidateTeamOnUpdate(team);
            UnitOfWork.Teams.Update(team);

            var group = await UnitOfWork.Groups
                .Query()
                .Include(x => x.Organization)
                .Where(x => x.Id == team.GroupId)
                .FirstOrDefaultAsync();

            group.HasActiveTeam = await UnitOfWork.Teams
                .QueryNoTracking()
                .AnyAsync(x => x.GroupId == group.Id && x.IsActive);

            group.Organization.HasActiveGroup = await UnitOfWork.Groups
                .QueryNoTracking()
                .AnyAsync(x => x.OrganizationId == group.Organization.Id && x.IsActive && x.HasActiveTeam);

            UnitOfWork.Groups.Update(group);

            await UnitOfWork.SaveChangesAsync();

            return new UpdateTeamStatusResponse();
        });

    public Task<bool> TeamExists(string name, int groupId) =>
        TryCatch(async () => {
            return await UnitOfWork.Teams.AnyAsync(x => x.Name == name && x.GroupId == groupId);
        });

    public async Task DeleteTeamAsync(int id) {
        await TryCatch(async () => {
            var team = await UnitOfWork.Teams.GetByIdAsync(id);

            if (team == null) {
                throw new TeamNotFoundException();
            }

            UnitOfWork.Teams.Remove(team);
            await UnitOfWork.SaveChangesAsync();

            return new UpdateTeamStatusResponse();
        });
    }

    public async Task<IEnumerable<GetTeamResponse>> GetAllTeamsAsync() {
        return await UnitOfWork.Teams
            .QueryNoTracking()
            .Select(team => new GetTeamResponse {
                Id = team.Id,
                Name = team.Name,
                GroupId = team.GroupId
            }).ToListAsync();
    }
}