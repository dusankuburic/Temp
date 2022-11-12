using Temp.Database;
using Temp.Domain.Models;
using Temp.Services.Teams.Models.Command;
using Temp.Services.Teams.Models.Query;

namespace Temp.Services.Teams;

public partial class TeamService : ITeamService
{
    private readonly ApplicationDbContext _ctx;
    private readonly IMapper _mapper;

    public TeamService(ApplicationDbContext ctx, IMapper mapper) {
        _ctx = ctx;
        _mapper = mapper;
    }

    public Task<CreateTeam.Response> CreateTeam(CreateTeam.Request request) {
        return TryCatch(async () => {
            var teamExists = await TeamExists(request.Name, request.GroupId);

            if (teamExists) {
                return new CreateTeam.Response {
                    Message = $"Error {request.Name} already exists",
                    Status = false
                };
            }

            var team = new Team
            {
                Name = request.Name,
                GroupId = request.GroupId
            };

            ValidateTeamOnCreate(team);

            _ctx.Teams.Add(team);
            await _ctx.SaveChangesAsync();

            return new CreateTeam.Response {
                Message = $"Success {team.Name} is added",
                Status = true
            };
        });
    }

    public Task<GetFullTeamTree.TeamTreeViewModel> GetFullTeamTree(int id) {
        return TryCatch(async () => {
            var team = await _ctx.Teams
            .AsNoTracking()
            .Include(x => x.Group)
            .Include(x => x.Group.Organization)
            .Where(x => x.Id == id && x.IsActive)
            .FirstOrDefaultAsync();

            return new GetFullTeamTree.TeamTreeViewModel {
                Id = team.Id,
                OrganizationName = team.Group.Organization.Name,
                OrganizationId = team.Group.Organization.Id,
                GroupName = team.Group.Name,
                TeamName = team.Name
            };
        });
    }

    public Task<GetTeam.TeamViewModel> GetTeam(int id) {
        return TryCatch(async () => {
            var team = await _ctx.Teams
            .AsNoTracking()
            .Include(x => x.Group)
            .Where(x => x.Id == id && x.IsActive)
            .Select(x => new GetTeam.TeamViewModel
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

    public Task<GetUserTeam.TeamViewModel> GetUserTeam(int id) {
        return TryCatch(async () => {
            var team = await _ctx.Users
            .AsNoTracking()
            .Include(x => x.Employee)
            .Where(x => x.Id == id && x.IsActive)
            .Select(x => new GetUserTeam.TeamViewModel
            {
                Id = x.Employee.Team.Id,
                Name = x.Employee.Team.Name
            })
            .FirstOrDefaultAsync();

            ValidateGetUserTeamViewModel(team);

            return team;
        });
    }

    public Task<UpdateTeam.Response> UpdateTeam(int id, UpdateTeam.Request request) {
        return TryCatch(async () => {
            var team = _ctx.Teams.FirstOrDefault(x => x.Id == id);

            if (team.Name.Equals(request.Name)) {
                return new UpdateTeam.Response {
                    Id = team.Id,
                    Name = team.Name,
                    Message = "Team name is same",
                    Status = true
                };
            }

            var teamExists = await TeamExists(request.Name, request.GroupId);

            if (teamExists) {
                return new UpdateTeam.Response {
                    Message = $"{request.Name} already exists",
                    Status = false
                };
            }

            team.Name = request.Name;

            ValidateTeamOnUpdate(team);
            await _ctx.SaveChangesAsync();

            return new UpdateTeam.Response {
                Id = team.Id,
                Name = team.Name,
                Message = "Success",
                Status = true
            };
        });
    }

    public async Task<bool> UpdateTeamStatus(int teamId) {
        var team = await _ctx.Teams
                .Where(x => x.Id == teamId)
                .FirstOrDefaultAsync();

        team.IsActive = false;

        await _ctx.SaveChangesAsync();

        return true;
    }

    private async Task<bool> TeamExists(string name, int groupId) {
        return await _ctx.Teams.AnyAsync(x => x.Name == name && x.GroupId == groupId);
    }
}

