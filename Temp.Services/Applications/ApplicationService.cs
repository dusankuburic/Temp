using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Temp.Database;
using Temp.Domain.Models.Applications;
using Temp.Services.Applications.CLI.Command;
using Temp.Services.Applications.CLI.Query;

namespace Temp.Services.Applications;

public partial class ApplicationService : IApplicationService
{
    private readonly ApplicationDbContext _ctx;
    private readonly IMapper _mapper;

    public ApplicationService(ApplicationDbContext ctx, IMapper mapper) {
        _ctx = ctx;
        _mapper = mapper;
    }

    public Task<CreateApplication.Response> CreateApplication(CreateApplication.Request request) =>
    TryCatch(async () => {
        var application = _mapper.Map<Application>(request);
        ValidateApplicationOnCreate(application);
        return await new CreateApplication(_ctx).Do(application);
    });

    public Task<UpdateApplicationStatus.Response> UpdateApplicationStatus(int id, UpdateApplicationStatus.Request request) =>
    TryCatch(async () => {
        var res = await _ctx.Applications
                .Where(x => x.Id == id)
                .FirstOrDefaultAsync();
        ValidateApplication(res);
        return await new UpdateApplicationStatus(_ctx).Do(res, request);
    });

    public Task<GetApplication.ApplicationViewModel> GetApplication(int id) =>
    TryCatch(async () => {
        var res = await new GetApplication(_ctx).Do(id);
        ValidateGetApplicationViewModel(res);
        return res;
    });

    public Task<IEnumerable<GetUserApplications.ApplicationViewModel>> GetUserApplications(int id) =>
    TryCatch(async () => {
        var res = await new GetUserApplications(_ctx).Do(id);
        ValidateGetUserApplicationsViewModel(res);
        return res;
    });

    public Task<IEnumerable<GetTeamApplications.ApplicationViewModel>> GetTeamApplications(int teamId, int moderatorId) =>
    TryCatch(async () => {
        var res = await new GetTeamApplications(_ctx).Do(teamId, moderatorId);
        ValidateGetTeamApplicationsViewModel(res);
        return res;
    });
}

