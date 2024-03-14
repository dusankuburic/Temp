using Temp.Database;
using Temp.Domain.Models;
using Temp.Services.Engagements.Models.Commands;
using Temp.Services.Engagements.Models.Queries;
using Temp.Services.Integrations.Loggings;

namespace Temp.Services.Engagements;

public partial class EngagementService : IEngagementService
{
    private readonly ApplicationDbContext _ctx;
    private readonly IMapper _mapper;
    private readonly ILoggingBroker _loggingBroker;

    public EngagementService(
        ApplicationDbContext ctx,
        IMapper mapper,
        ILoggingBroker loggingBroker) {
        _ctx = ctx;
        _mapper = mapper;
        _loggingBroker = loggingBroker;
    }

    public Task<CreateEngagementResponse> CreateEngagement(CreateEngagementRequest request) =>
        TryCatch(async () => {
            var engagement = _mapper.Map<Engagement>(request);

            ValidateEngagementOnCreate(engagement);

            _ctx.Engagements.Add(engagement);
            await _ctx.SaveChangesAsync();

            return _mapper.Map<CreateEngagementResponse>(engagement);
        });

    public Task<List<GetUserEmployeeEngagementsResponse>> GetUserEmployeeEngagements(GetUserEmployeeEngagementsRequest request) =>
        TryCatch(async () => {
            var userEmployeeId = await _ctx.Employees
                .Where(x => x.Id == request.Id)
                .Select(x => x.Id)
                .FirstOrDefaultAsync();

            var engagements = await _ctx.Engagements
                .Include(x => x.Workplace)
                .Include(x => x.EmploymentStatus)
                .Where(x => x.EmployeeId == userEmployeeId)
                .ProjectTo<GetUserEmployeeEngagementsResponse>(_mapper.ConfigurationProvider)
                .ToListAsync();

            return engagements;
        });

    public Task<List<GetEngagementsForEmployeeResponse>> GetEngagementForEmployee(GetEngagementsForEmployeeRequest request) =>
        TryCatch(async () => {
            var engagements = await _ctx.Engagements
                .Include(x => x.Workplace)
                .Include(x => x.EmploymentStatus)
                .Where(x => x.EmployeeId == request.Id)
                .ProjectTo<GetEngagementsForEmployeeResponse>(_mapper.ConfigurationProvider)
                .ToListAsync();

            return engagements;
        });
}
