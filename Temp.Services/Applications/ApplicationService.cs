using Temp.Database.UnitOfWork;
using Temp.Domain.Models.Applications;
using Temp.Services._Shared;
using Temp.Services.Applications.Models.Commands;
using Temp.Services.Applications.Models.Queries;
using Temp.Services.Integrations.Loggings;
using Temp.Services.Providers;

namespace Temp.Services.Applications;

public partial class ApplicationService : BaseService<Application>, IApplicationService
{
    public ApplicationService(
        IUnitOfWork unitOfWork,
        IMapper mapper,
        ILoggingBroker loggingBroker,
        IIdentityProvider identityProvider)
        : base(unitOfWork, mapper, loggingBroker, identityProvider) {
    }

    public Task<CreateApplicationResponse> CreateApplication(CreateApplicationRequest request) =>
    TryCatch(async () => {
        var application = Mapper.Map<Application>(request);

        application.SetAuditableInfoOnCreate(await IdentityProvider.GetCurrentUser());

        ValidateApplicationOnCreate(application);

        application.Status = false;
        await UnitOfWork.Applications.AddAsync(application);
        await UnitOfWork.SaveChangesAsync();

        return Mapper.Map<CreateApplicationResponse>(application);
    });

    public Task<UpdateApplicationStatusResponse> UpdateApplicationStatus(UpdateApplicationStatusRequest request) =>
    TryCatch(async () => {
        var application = await UnitOfWork.Applications
            .FirstOrDefaultAsync(x => x.Id == request.Id);

        application.SetAuditableInfoOnUpdate(await IdentityProvider.GetCurrentUser());

        ValidateApplication(application);

        Mapper.Map(request, application);

        UnitOfWork.Applications.Update(application);
        await UnitOfWork.SaveChangesAsync();

        return new UpdateApplicationStatusResponse {
            Id = application.Id,
            Success = true
        };
    });

    public Task<GetApplicationResponse> GetApplication(GetApplicationRequest request) =>
    TryCatch(async () => {
        var application = await UnitOfWork.Applications
            .QueryNoTracking()
            .Where(x => x.Id == request.Id)
            .ProjectTo<GetApplicationResponse>(Mapper.ConfigurationProvider)
            .FirstOrDefaultAsync();

        ValidateGetApplication(application);

        return application;
    });

    public Task<IEnumerable<GetUserApplicationsResponse>> GetUserApplications(GetUserApplicationsRequest request) =>
    TryCatch(async () => {
        var applications = await UnitOfWork.Applications
            .QueryNoTracking()
            .Where(x => x.UserId == request.Id)
            .OrderByDescending(x => x.CreatedAt)
            .ThenBy(x => x.Status)
            .ProjectTo<GetUserApplicationsResponse>(Mapper.ConfigurationProvider)
            .ToListAsync();

        return applications;
    });

    public Task<IEnumerable<GetTeamApplicationsResponse>> GetTeamApplications(GetTeamApplicationsRequest request) =>
    TryCatch(async () => {
        var applications = await UnitOfWork.Applications
            .QueryNoTracking()
            .Where(x => x.TeamId == request.TeamId)

            .OrderBy(x => x.Status)
            .ProjectTo<GetTeamApplicationsResponse>(Mapper.ConfigurationProvider)
            .ToListAsync();

        return applications;
    });
}