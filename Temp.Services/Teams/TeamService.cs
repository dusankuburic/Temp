using Temp.Database;
using Temp.Domain.Models;
using Temp.Services.Teams.Models.Commands;
using Temp.Services.Teams.Models.Queries;

namespace Temp.Services.Teams;

public partial class TeamService : ITeamService
{
    private readonly ApplicationDbContext _ctx;
    private readonly IMapper _mapper;

    public TeamService(ApplicationDbContext ctx, IMapper mapper) {
        _ctx = ctx;
        _mapper = mapper;
    }

    public Task<CreateTeamResponse> CreateTeam(CreateTeamRequest request) {
        return TryCatch(async () => {
            var team = new Team
            {
                Name = request.Name,
                GroupId = request.GroupId
            };

            ValidateTeamOnCreate(team);

            _ctx.Teams.Add(team);
            await _ctx.SaveChangesAsync();

            return new CreateTeamResponse {
                Id = team.Id,
                Name = team.Name,
                GroupId = team.GroupId
            };
        });
    }

    public Task<GetFullTeamTreeResponse> GetFullTeamTree(GetFullTeamTreeRequest requst) {
        return TryCatch(async () => {
            var team = await _ctx.Teams
                .AsNoTracking()
                .Include(x => x.Group)
                .Include(x => x.Group.Organization)
                .Where(x => x.Id == requst.Id && x.IsActive)
                .FirstOrDefaultAsync();

            return new GetFullTeamTreeResponse {
                Id = team.Id,
                OrganizationName = team.Group.Organization.Name,
                OrganizationId = team.Group.Organization.Id,
                GroupName = team.Group.Name,
                TeamName = team.Name
            };
        });
    }

    public Task<GetTeamResponse> GetTeam(GetTeamRequest request) {
        return TryCatch(async () => {
            var team = await _ctx.Teams
                .AsNoTracking()
                .Include(x => x.Group)
                .Where(x => x.Id == request.Id && x.IsActive)
                .Select(x => new GetTeamResponse
                {
                    Id = x.Id,
                    Name = x.Name,
                    GroupId = x.Group.Id
                })
                .FirstOrDefaultAsync();

            ValidateGetTeamViewModel(team);

            return team;
        });
    }

    public Task<GetUserTeamResponse> GetUserTeam(GetUserTeamRequest request) {
        return TryCatch(async () => {
            var team = await _ctx.Users
                .AsNoTracking()
                .Include(x => x.Employee)
                .Where(x => x.Id == request.Id && x.IsActive)
                .Select(x => new GetUserTeamResponse
                {
                    Id = x.Employee.Team.Id,
                    Name = x.Employee.Team.Name
                })
                .FirstOrDefaultAsync();

            ValidateGetUserTeamViewModel(team);

            return team;
        });
    }

    public Task<UpdateTeamResponse> UpdateTeam(UpdateTeamRequest request) {
        return TryCatch(async () => {
            var team = _ctx.Teams.FirstOrDefault(x => x.Id == request.Id);

            team.Name = request.Name;

            ValidateTeamOnUpdate(team);
            await _ctx.SaveChangesAsync();

            return new UpdateTeamResponse {
                Success = true
            };
        });
    }

    public async Task<UpdateTeamStatusResponse> UpdateTeamStatus(UpdateTeamStatusRequest request) {
        var team = await _ctx.Teams
                .Where(x => x.Id == request.Id)
                .FirstOrDefaultAsync();

        team.IsActive = !team.IsActive;

        await _ctx.SaveChangesAsync();

        return new UpdateTeamStatusResponse {
            Success = true
        };
    }

    private async Task<bool> TeamExists(string name, int groupId) {
        return await _ctx.Teams.AnyAsync(x => x.Name == name && x.GroupId == groupId);
    }
}

