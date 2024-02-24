﻿using AutoMapper.QueryableExtensions;
using Temp.Database;
using Temp.Domain.Models;
using Temp.Services.Integrations.Loggings;
using Temp.Services.Teams.Models.Commands;
using Temp.Services.Teams.Models.Queries;

namespace Temp.Services.Teams;

public partial class TeamService : ITeamService
{
    private readonly ApplicationDbContext _ctx;
    private readonly IMapper _mapper;
    private readonly ILoggingBroker _loggingBroker;

    public TeamService(
        ApplicationDbContext ctx,
        IMapper mapper,
        ILoggingBroker loggingBroker) {
        _ctx = ctx;
        _mapper = mapper;
        _loggingBroker = loggingBroker;
    }

    public Task<CreateTeamResponse> CreateTeam(CreateTeamRequest request) {
        return TryCatch(async () => {
            var team = _mapper.Map<Team>(request);

            ValidateTeamOnCreate(team);

            _ctx.Teams.Add(team);
            await _ctx.SaveChangesAsync();

            return _mapper.Map<CreateTeamResponse>(team);
        });
    }

    public Task<GetFullTeamTreeResponse> GetFullTeamTree(GetFullTeamTreeRequest requst) {
        return TryCatch(async () => {
            var team = await _ctx.Teams
                .Include(x => x.Group)
                .ThenInclude(x => x.Organization)
                .Where(x => x.Id == requst.Id && x.IsActive)
                .ProjectTo<GetFullTeamTreeResponse>(_mapper.ConfigurationProvider)
                .FirstOrDefaultAsync();

            return team;
        });
    }

    public Task<GetTeamResponse> GetTeam(GetTeamRequest request) {
        return TryCatch(async () => {
            var team = await _ctx.Teams
                .Include(x => x.Group)
                .Where(x => x.Id == request.Id && x.IsActive)
                .ProjectTo<GetTeamResponse>(_mapper.ConfigurationProvider)
                .FirstOrDefaultAsync();

            ValidateGetTeam(team);

            return team;
        });
    }

    public Task<GetUserTeamResponse> GetUserTeam(GetUserTeamRequest request) {
        return TryCatch(async () => {
            var team = await _ctx.Users
                .Include(x => x.Employee)
                .ThenInclude(x => x.Team)
                .Where(x => x.Id == request.Id && x.IsActive)
                .ProjectTo<GetUserTeamResponse>(_mapper.ConfigurationProvider)
                .FirstOrDefaultAsync();

            ValidateGetUserTeam(team);

            return team;
        });
    }

    public Task<UpdateTeamResponse> UpdateTeam(UpdateTeamRequest request) {
        return TryCatch(async () => {
            var team = await _ctx.Teams
                .Where(x => x.Id == request.Id)
                .FirstOrDefaultAsync();

            _mapper.Map(request, team);

            ValidateTeamOnUpdate(team);
            await _ctx.SaveChangesAsync();

            return new UpdateTeamResponse();
        });
    }

    public async Task<UpdateTeamStatusResponse> UpdateTeamStatus(UpdateTeamStatusRequest request) {
        var team = await _ctx.Teams
                .Where(x => x.Id == request.Id)
                .FirstOrDefaultAsync();

        team.IsActive = !team.IsActive;

        await _ctx.SaveChangesAsync();

        return new UpdateTeamStatusResponse();
    }

    private async Task<bool> TeamExists(string name, int groupId) {
        return await _ctx.Teams.AnyAsync(x => x.Name == name && x.GroupId == groupId);
    }
}

