using Temp.Database;
using Temp.Domain.Models;
using Temp.Services.Engagements.Models.Commands;
using Temp.Services.Engagements.Models.Queries;

namespace Temp.Services.Engagements;

public partial class EngagementService : IEngagementService
{
    private readonly ApplicationDbContext _ctx;

    public EngagementService(ApplicationDbContext ctx) {
        _ctx = ctx;
    }

    public Task<CreateEngagementResponse> CreateEngagement(CreateEngagementRequest request) =>
        TryCatch(async () => {
            var engagement = new Engagement {
                EmployeeId = request.EmployeeId,
                WorkplaceId = request.WorkplaceId,
                EmploymentStatusId = request.EmploymentStatusId,
                DateFrom = request.DateFrom,
                DateTo = request.DateTo,
                Salary = request.Salary
            };

            ValidateEngagementOnCreate(engagement);

            _ctx.Engagements.Add(engagement);
            await _ctx.SaveChangesAsync();

            return new CreateEngagementResponse {
                Id = engagement.Id,
                EmployeeId = request.EmployeeId,
                WorkplaceId = request.WorkplaceId,
                EmploymentStatusId = request.EmploymentStatusId,
                DateFrom = request.DateFrom,
                DateTo = request.DateTo,
                Salary = request.Salary
            };
        });

    public Task<List<GetUserEmployeeEngagementsResponse>> GetUserEmployeeEngagements(GetUserEmployeeEngagementsRequest request) =>
        TryCatch(async () => {
            var userEmployeeId = await _ctx.Users
                .Include(x => x.Employee)
                .Where(x => x.Id == request.Id)
                .Select(x => x.EmployeeId)
                .FirstOrDefaultAsync();

            var engagements = await _ctx.Engagements
                .Include(x => x.Workplace)
                .Include(x => x.EmploymentStatus)
                .Where(x => x.EmployeeId == userEmployeeId)
                .Select(x => new GetUserEmployeeEngagementsResponse {
                    WorkplaceName = x.Workplace.Name,
                    EmploymentStatusName = x.EmploymentStatus.Name,
                    DateFrom = x.DateFrom,
                    DateTo = x.DateTo,
                    Salary = x.Salary
                })
                .AsNoTracking()
                .ToListAsync();

            return engagements;
        });

    public Task<List<GetEngagementsForEmployeeResponse>> GetEngagementForEmployee(GetEngagementsForEmployeeRequest request) =>
        TryCatch(async () => {
            var engagements = await _ctx.Engagements
                .Include(x => x.Workplace)
                .Include(x => x.EmploymentStatus)
                .Where(x => x.EmployeeId == request.Id)
                .Select(x => new GetEngagementsForEmployeeResponse {
                    Id = x.Id,
                    WorkplaceName = x.Workplace.Name,
                    EmploymentStatusName = x.EmploymentStatus.Name,
                    DateFrom = x.DateFrom,
                    DateTo = x.DateTo,
                    Salary = x.Salary
                })
                .AsNoTracking()
                .ToListAsync();

            return engagements;
        });
}
