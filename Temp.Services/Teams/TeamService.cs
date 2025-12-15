using Temp.Database.UnitOfWork;
using Temp.Domain.Models;
using Temp.Services.Integrations.Loggings;
using Temp.Services.Providers;
using Temp.Services.Teams.Exceptions;
using Temp.Services.Teams.Models.Commands;
using Temp.Services.Teams.Models.Queries;

namespace Temp.Services.Teams;

public partial class TeamService : ITeamService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ILoggingBroker _loggingBroker;
    private readonly IIdentityProvider _identityProvider;

    public TeamService(
        IUnitOfWork unitOfWork,
        IMapper mapper,
        ILoggingBroker loggingBroker,
        IIdentityProvider identityProvider) {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _loggingBroker = loggingBroker;
        _identityProvider = identityProvider;
    }

    public Task<CreateTeamResponse> CreateTeam(CreateTeamRequest request) =>
         TryCatch(async () => {
             var team = _mapper.Map<Team>(request);

             team.SetAuditableInfoOnCreate(await _identityProvider.GetCurrentUser());

             ValidateTeamOnCreate(team);

             await _unitOfWork.Teams.AddAsync(team);
             await _unitOfWork.SaveChangesAsync();

             var group = await _unitOfWork.Groups
                .Query()
                .Include(x => x.Organization)
                .Where(x => x.Id == request.GroupId)
                .FirstOrDefaultAsync();

             group.HasActiveTeam = true;
             group.Organization.HasActiveGroup = true;

             await _unitOfWork.SaveChangesAsync();

             return _mapper.Map<CreateTeamResponse>(team);
         });

    public Task<GetFullTeamTreeResponse> GetFullTeamTree(GetFullTeamTreeRequest requst) =>
        TryCatch(async () => {
            var team = await _unitOfWork.Teams
                .Query()
                .Include(x => x.Group)
                .ThenInclude(x => x.Organization)
                .Where(x => x.Id == requst.Id && x.IsActive)
                .ProjectTo<GetFullTeamTreeResponse>(_mapper.ConfigurationProvider)
                .FirstOrDefaultAsync();

            return team;
        });

    public Task<GetTeamResponse> GetTeam(GetTeamRequest request) =>
       TryCatch(async () => {
           var team = await _unitOfWork.Teams
                .Query()
                .Include(x => x.Group)
                .Where(x => x.Id == request.Id && x.IsActive)
                .ProjectTo<GetTeamResponse>(_mapper.ConfigurationProvider)
                .FirstOrDefaultAsync();

           ValidateGetTeam(team);

           return team;
       });

    public Task<GetUserTeamResponse> GetUserTeam(GetUserTeamRequest request) =>
        TryCatch(async () => {
            var team = await _unitOfWork.Employees
                .Query()
                .Include(x => x.Team)
                .Where(x => x.Id == request.Id)
                .ProjectTo<GetUserTeamResponse>(_mapper.ConfigurationProvider)
                .FirstOrDefaultAsync();

            ValidateGetUserTeam(team);

            return team;
        });

    public Task<UpdateTeamResponse> UpdateTeam(UpdateTeamRequest request) =>
        TryCatch(async () => {
            var team = await _unitOfWork.Teams
                .FirstOrDefaultAsync(x => x.Id == request.Id);

            _mapper.Map(request, team);

            team.SetAuditableInfoOnUpdate(await _identityProvider.GetCurrentUser());

            ValidateTeamOnUpdate(team);
            _unitOfWork.Teams.Update(team);
            await _unitOfWork.SaveChangesAsync();

            return new UpdateTeamResponse();
        });

    public Task<UpdateTeamStatusResponse> UpdateTeamStatus(UpdateTeamStatusRequest request) =>
        TryCatch(async () => {
            var team = await _unitOfWork.Teams
                .FirstOrDefaultAsync(x => x.Id == request.Id);

            team.IsActive = !team.IsActive;
            team.SetAuditableInfoOnUpdate(await _identityProvider.GetCurrentUser());

            ValidateTeamOnUpdate(team);
            _unitOfWork.Teams.Update(team);
            await _unitOfWork.SaveChangesAsync();

            var group = await _unitOfWork.Groups
                .Query()
                .Include(x => x.Organization)
                .Where(x => x.Id == team.GroupId)
                .FirstOrDefaultAsync();

            group.HasActiveTeam = await _unitOfWork.Groups
                .Query()
                .Include(x => x.Teams)
                .Where(x => x.Id == group.Id)
                .AnyAsync(x => x.Teams.Any(x => x.IsActive));

            group.Organization.HasActiveGroup = await _unitOfWork.Organizations
                .Query()
                .Include(x => x.Groups)
                .Where(x => x.Id == group.Organization.Id)
                .AnyAsync(x => x.Groups.Any(x => x.IsActive && x.HasActiveTeam));
            _unitOfWork.Groups.Update(group);
            await _unitOfWork.SaveChangesAsync();

            return new UpdateTeamStatusResponse();
        });

    public Task<bool> TeamExists(string name, int groupId) =>
        TryCatch(async () => {
            return await _unitOfWork.Teams.AnyAsync(x => x.Name == name && x.GroupId == groupId);
        });

    public async Task DeleteTeamAsync(int id)
    {
        await TryCatch(async () =>
        {
            var team = await _unitOfWork.Teams.GetByIdAsync(id);

            if (team == null)
            {
                throw new TeamNotFoundException();
            }

            _unitOfWork.Teams.Remove(team);
            await _unitOfWork.SaveChangesAsync();

            return new UpdateTeamStatusResponse();
        });
    }

    public async Task<IEnumerable<GetTeamResponse>> GetAllTeamsAsync()
    {
        return await _unitOfWork.Teams
            .QueryNoTracking()
            .Select(team => new GetTeamResponse
            {
                Id = team.Id,
                Name = team.Name,
                GroupId = team.GroupId
            }).ToListAsync();
    }

}

