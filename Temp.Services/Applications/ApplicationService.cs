using AutoMapper.QueryableExtensions;
using Temp.Database;
using Temp.Domain.Models.Applications;
using Temp.Services.Applications.Models.Commands;
using Temp.Services.Applications.Models.Queries;

namespace Temp.Services.Applications;

public partial class ApplicationService : IApplicationService
{
    private readonly ApplicationDbContext _ctx;
    private readonly IMapper _mapper;

    public ApplicationService(ApplicationDbContext ctx, IMapper mapper) {
        _ctx = ctx;
        _mapper = mapper;
    }

    public Task<CreateApplicationResponse> CreateApplication(CreateApplicationRequest request) =>
    TryCatch(async () => {
        var application = _mapper.Map<Application>(request);
        ValidateApplicationOnCreate(application);

        _ctx.Applications.Add(application);
        await _ctx.SaveChangesAsync();

        return _mapper.Map<CreateApplicationResponse>(application);
    });

    public Task<UpdateApplicationStatusResponse> UpdateApplicationStatus(UpdateApplicationStatusRequest request) =>
    TryCatch(async () => {
        var application = await _ctx.Applications
            .Where(x => x.Id == request.Id)
            .FirstOrDefaultAsync();

        ValidateApplication(application);

        _mapper.Map(request, application);

        await _ctx.SaveChangesAsync();

        return new UpdateApplicationStatusResponse {
            Id = application.Id,
            Success = true
        };
    });

    public Task<GetApplicationResponse> GetApplication(GetApplicationRequest request) =>
    TryCatch(async () => {
        var application = await _ctx.Applications
            .AsNoTracking()
            .Where(x => x.Id == request.Id)
            .ProjectTo<GetApplicationResponse>(_mapper.ConfigurationProvider)
            .FirstOrDefaultAsync();

        ValidateGetApplication(application);

        return application;
    });

    public Task<IEnumerable<GetUserApplicationsResponse>> GetUserApplications(GetUserApplicationsRequest request) =>
    TryCatch(async () => {
        var applications = await _ctx.Applications
            .AsNoTracking()
            .Where(x => x.UserId == request.Id)
            .OrderByDescending(x => x.CreatedAt)
            .ThenBy(x => x.Status)
            .ProjectTo<GetUserApplicationsResponse>(_mapper.ConfigurationProvider)
            .ToListAsync();

        ValidateGetUserApplications(applications);

        return applications;
    });

    public Task<IEnumerable<GetTeamApplicationsResponse>> GetTeamApplications(GetTeamApplicationsRequest request) =>
    TryCatch(async () => {
        var applications = await _ctx.Applications
            .AsNoTracking()
            .Include(x => x.User)
            .Where(x => x.TeamId == request.TeamId)
            .Where(x => (x.ModeratorId == request.ModeratorId) || (x.Status == false))
            .OrderBy(x => x.Status)
            .ProjectTo<GetTeamApplicationsResponse>(_mapper.ConfigurationProvider)
            .ToListAsync();

        ValidateGetTeamApplicationsViewModel(applications);

        return applications;
    });
}

