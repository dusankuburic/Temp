using Temp.Database.UnitOfWork;
using Temp.Domain.Models;
using Temp.Services._Helpers;
using Temp.Services._Shared;
using Temp.Services.Integrations.Azure.AzureStorage;
using Temp.Services.Integrations.Loggings;
using Temp.Services.Providers;
using Temp.Services.Workplaces.Models.Commands;
using Temp.Services.Workplaces.Models.Queries;

namespace Temp.Services.Workplaces;

public partial class WorkplaceService : BaseService<Workplace>, IWorkplaceService
{
    private readonly IAzureStorageService _azureStorageService;

    public WorkplaceService(
        IUnitOfWork unitOfWork,
        IMapper mapper,
        ILoggingBroker loggingBroker,
        IIdentityProvider identityProvider,
        IAzureStorageService azureStorageService)
        : base(unitOfWork, mapper, loggingBroker, identityProvider) {
        _azureStorageService = azureStorageService;
    }

    public Task<CreateWorkplaceResponse> CreateWorkplace(CreateWorkplaceRequest request) =>
         TryCatch(async () => {
             var workplace = Mapper.Map<Workplace>(request);

             workplace.SetAuditableInfoOnCreate(await IdentityProvider.GetCurrentUser());

             ValidateWorkplaceOnCreate(workplace);

             await UnitOfWork.Workplaces.AddAsync(workplace);
             await UnitOfWork.SaveChangesAsync();

             return Mapper.Map<CreateWorkplaceResponse>(workplace);
         });

    public Task<PagedList<GetWorkplacesResponse>> GetPagedWorkplaces(GetWorkplacesRequest request) =>
         TryCatch(async () => {
             var workplacesQuery = UnitOfWork.Workplaces
                .QueryNoTracking()
                .Where(x => x.IsActive)
                .ProjectTo<GetWorkplacesResponse>(Mapper.ConfigurationProvider);

             if (!string.IsNullOrEmpty(request.Name)) {
                 workplacesQuery = workplacesQuery.Where(x => x.Name.Contains(request.Name));
             }

             workplacesQuery = workplacesQuery.OrderBy(x => x.Name);

             return await PagedList<GetWorkplacesResponse>.CreateAsync(
                 workplacesQuery,
                 request.PageNumber,
                 request.PageSize);
         });

    public Task<List<GetWorkplaceResponse>> GetWorkplaces() =>
        TryCatch(async () => {
            var workplaces = await UnitOfWork.Workplaces
                .QueryNoTracking()
                .Where(x => x.IsActive)
                .OrderBy(x => x.Name)
                .ProjectTo<GetWorkplaceResponse>(Mapper.ConfigurationProvider)
                .ToListAsync();

            return workplaces;
        });

    public Task<GetWorkplaceResponse> GetWorkplace(int id) =>
        TryCatch(async () => {
            var workplace = await UnitOfWork.Workplaces
                .QueryNoTracking()
                .Where(x => x.IsActive && x.Id == id)
                .ProjectTo<GetWorkplaceResponse>(Mapper.ConfigurationProvider)
                .FirstOrDefaultAsync();

            return workplace;
        });

    public Task<UpdateWorkplaceResponse> UpdateWorkplace(UpdateWorkplaceRequest request) =>
        TryCatch(async () => {
            var workplace = await UnitOfWork.Workplaces
                .FirstOrDefaultAsync(x => x.Id == request.Id);

            string? oldProfilePictureUrl = workplace.ProfilePictureUrl;

            Mapper.Map(request, workplace);

            workplace.SetAuditableInfoOnUpdate(await IdentityProvider.GetCurrentUser());

            ValidateWorkplaceOnUpdate(workplace);

            UnitOfWork.Workplaces.Update(workplace);
            await UnitOfWork.SaveChangesAsync();

            if (!string.IsNullOrEmpty(oldProfilePictureUrl) &&
                oldProfilePictureUrl != request.ProfilePictureUrl) {
                try {
                    await _azureStorageService.DeleteAsync(oldProfilePictureUrl);
                } catch (Exception ex) {
                    Logger.LogError(ex);
                }
            }

            return new UpdateWorkplaceResponse();
        });

    public Task<UpdateWorkplaceStatusResponse> UpdateWorkplaceStatus(UpdateWorkplaceStatusRequest request) =>
        TryCatch(async () => {
            var workplace = await UnitOfWork.Workplaces
                .FirstOrDefaultAsync(x => x.Id == request.Id);

            workplace.IsActive = !workplace.IsActive;
            workplace.SetAuditableInfoOnUpdate(await IdentityProvider.GetCurrentUser());

            ValidateWorkplaceOnStatusUpdate(workplace);

            UnitOfWork.Workplaces.Update(workplace);
            await UnitOfWork.SaveChangesAsync();

            return new UpdateWorkplaceStatusResponse();
        });

    public Task<bool> WorkplaceExists(string name) =>
        TryCatch(async () => {
            return await UnitOfWork.Workplaces.AnyAsync(x => x.Name == name);
        });
}