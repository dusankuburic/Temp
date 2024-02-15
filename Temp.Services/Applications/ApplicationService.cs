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

        return new CreateApplicationResponse {
            Id = application.Id,
            UserId = application.UserId,
            TeamId = application.TeamId,
            Content = application.Content,
            Category = application.Category
        };
    });

    public Task<UpdateApplicationStatusResponse> UpdateApplicationStatus(UpdateApplicationStatusRequest request) =>
    TryCatch(async () => {
        var res = await _ctx.Applications
                .Where(x => x.Id == request.Id)
                .FirstOrDefaultAsync();
        ValidateApplication(res);

        res.ModeratorId = request.ModeratorId;
        res.Status = true;
        res.StatusUpdatedAt = DateTime.Now;

        await _ctx.SaveChangesAsync();

        return new UpdateApplicationStatusResponse {
            Id = res.Id,
            Success = true
        };
    });

    public Task<GetApplicationResponse> GetApplication(GetApplicationRequest request) =>
    TryCatch(async () => {
        var res = await _ctx.Applications
               .AsNoTracking()
               .Where(x => x.Id == request.Id)
               .Select(x => new GetApplicationResponse
               {
                   Id = x.Id,
                   Category = x.Category,
                   Content = x.Content,
                   CreatedAt = x.CreatedAt
               })
               .FirstOrDefaultAsync();

        ValidateGetApplicationViewModel(res);

        return res;
    });

    public Task<IEnumerable<GetUserApplicationsResponse>> GetUserApplications(GetUserApplicationsRequest request) =>
    TryCatch(async () => {
        var res = await _ctx.Applications
            .AsNoTracking()
            .Where(x => x.UserId == request.Id)
            .OrderByDescending(x => x.CreatedAt)
            .ThenBy(x => x.Status)
            .Select(x => new GetUserApplicationsResponse
            {
                Id = x.Id,
                Category = x.Category,
                CreatedAt = x.CreatedAt,
                Status = x.Status
            })
            .ToListAsync();

        ValidateGetUserApplicationsViewModel(res);
        return res;
    });

    public Task<IEnumerable<GetTeamApplicationsResponse>> GetTeamApplications(GetTeamApplicationsRequest request) =>
    TryCatch(async () => {
        var res = await _ctx.Applications
            .AsNoTracking()
            .Include(x => x.User)
            .Where(x => x.TeamId == request.TeamId)
            .Where(x => (x.ModeratorId == request.ModeratorId) || (x.Status == false))
            .OrderBy(x => x.Status)
            .Select(x => new GetTeamApplicationsResponse
            {
                Id = x.Id,
                TeamId = x.TeamId,
                Username = x.User.Username,
                Category = x.Category,
                CreatedAt = x.CreatedAt,
                Status = x.Status
            })
            .ToListAsync();

        ValidateGetTeamApplicationsViewModel(res);

        return res;
    });
}

