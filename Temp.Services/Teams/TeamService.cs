using Temp.Database;
using Temp.Domain.Models;
using Temp.Services.Teams.CLI.Command;
using Temp.Services.Teams.CLI.Query;

namespace Temp.Services.Teams;

public partial class TeamService : ITeamService
{
    private readonly ApplicationDbContext _ctx;
    private readonly IMapper _mapper;

    public TeamService(ApplicationDbContext ctx, IMapper mapper) {
        _ctx = ctx;
        _mapper = mapper;
    }

    public Task<CreateTeam.Response> CreateTeam(CreateTeam.Request request) =>
    TryCatch(async () => {
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
        return await new CreateTeam(_ctx).Do(team);
    });


    public Task<GetFullTeamTree.TeamTreeViewModel> GetFullTeamTree(int id) =>
    TryCatch(async () => {
        return await new GetFullTeamTree(_ctx).Do(id);
    });


    public Task<GetTeam.TeamViewModel> GetTeam(int id) =>
    TryCatch(async () => {
        var team = await new GetTeam(_ctx).Do(id);
        ValidateGetTeamViewModel(team);
        return team;
    });

    public Task<GetUserTeam.TeamViewModel> GetUserTeam(int id) =>
    TryCatch(async () => {
        var team = await new GetUserTeam(_ctx).Do(id);
        ValidateGetUserTeamViewModel(team);
        return team;
    });

    public Task<UpdateTeam.Response> UpdateTeam(int id, UpdateTeam.Request request) =>
    TryCatch(async () => {
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
        return await new UpdateTeam(_ctx).Do(team);
    });

    public Task<bool> UpdateTeamStatus(int teamId) {
        throw new NotImplementedException();
    }

    private async Task<bool> TeamExists(string name, int groupId) {
        if (await _ctx.Teams.AnyAsync(x => x.Name == name && x.GroupId == groupId))
            return true;

        return false;
    }
}

