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

    public Task<CreateApplication.Response> CreateApplication(CreateApplication.Request request) =>
    TryCatch(async () => {
        var application = _mapper.Map<Application>(request);
        ValidateApplicationOnCreate(application);

        _ctx.Applications.Add(application);
        await _ctx.SaveChangesAsync();

        return new CreateApplication.Response {
            Status = true
        };
    });

    public Task<UpdateApplicationStatus.Response> UpdateApplicationStatus(int id, UpdateApplicationStatus.Request request) =>
    TryCatch(async () => {
        var res = await _ctx.Applications
                .Where(x => x.Id == id)
                .FirstOrDefaultAsync();
        ValidateApplication(res);

        res.ModeratorId = request.ModeratorId;
        res.Status = true;
        res.StatusUpdatedAt = DateTime.Now;

        await _ctx.SaveChangesAsync();

        return new UpdateApplicationStatus.Response {
            Id = res.Id,
            Status = true
        };
    });

    public Task<GetApplication.ApplicationViewModel> GetApplication(int id) =>
    TryCatch(async () => {
        var res = await _ctx.Applications
               .AsNoTracking()
               .Where(x => x.Id == id)
               .Select(x => new GetApplication.ApplicationViewModel
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

    public Task<IEnumerable<GetUserApplications.ApplicationViewModel>> GetUserApplications(int id) =>
    TryCatch(async () => {
        var res = await _ctx.Applications
            .AsNoTracking()
            .Where(x => x.UserId == id)
            .OrderByDescending(x => x.CreatedAt)
            .ThenBy(x => x.Status)
            .Select(x => new GetUserApplications.ApplicationViewModel
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

    public Task<IEnumerable<GetTeamApplications.ApplicationViewModel>> GetTeamApplications(int teamId, int moderatorId) =>
    TryCatch(async () => {
        var res = await _ctx.Applications
            .AsNoTracking()
            .Include(x => x.User)
            .Where(x => x.TeamId == teamId)
            .Where(x => (x.ModeratorId == moderatorId) || (x.Status == false))
            .OrderBy(x => x.Status)
            .Select(x => new GetTeamApplications.ApplicationViewModel
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

