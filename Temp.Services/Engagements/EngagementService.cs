using Temp.Database.UnitOfWork;
using Temp.Domain.Models;
using Temp.Services._Shared;
using Temp.Services.Engagements.Models.Commands;
using Temp.Services.Engagements.Models.Queries;
using Temp.Services.Integrations.Loggings;
using Temp.Services.Providers;

namespace Temp.Services.Engagements;

public partial class EngagementService : BaseService<Engagement>, IEngagementService
{
    public EngagementService(
        IUnitOfWork unitOfWork,
        IMapper mapper,
        ILoggingBroker loggingBroker,
        IIdentityProvider identityProvider)
        : base(unitOfWork, mapper, loggingBroker, identityProvider) {
    }

    public Task<CreateEngagementResponse> CreateEngagement(CreateEngagementRequest request) =>
        TryCatch(async () => {
            var engagement = Mapper.Map<Engagement>(request);

            engagement.SetAuditableInfoOnCreate(await IdentityProvider.GetCurrentUser());

            ValidateEngagementOnCreate(engagement);

            await UnitOfWork.Engagements.AddAsync(engagement);
            await UnitOfWork.SaveChangesAsync();

            return Mapper.Map<CreateEngagementResponse>(engagement);
        });

    public Task<List<GetUserEmployeeEngagementsResponse>> GetUserEmployeeEngagements(GetUserEmployeeEngagementsRequest request) =>
        TryCatch(async () => {
            var userEmployeeId = await UnitOfWork.Employees
                .QueryNoTracking()
                .Where(x => x.Id == request.Id)
                .Select(x => x.Id)
                .FirstOrDefaultAsync();

            var engagements = await UnitOfWork.Engagements
                .QueryNoTracking()
                .Where(x => x.EmployeeId == userEmployeeId)
                .ProjectTo<GetUserEmployeeEngagementsResponse>(Mapper.ConfigurationProvider)
                .ToListAsync();

            return engagements;
        });

    public Task<List<GetEngagementsForEmployeeResponse>> GetEngagementForEmployee(GetEngagementsForEmployeeRequest request) =>
        TryCatch(async () => {
            var engagements = await UnitOfWork.Engagements
                .QueryNoTracking()
                .Where(x => x.EmployeeId == request.Id)
                .ProjectTo<GetEngagementsForEmployeeResponse>(Mapper.ConfigurationProvider)
                .ToListAsync();

            return engagements;
        });
}