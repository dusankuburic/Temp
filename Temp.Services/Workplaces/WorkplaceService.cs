using Temp.Database.UnitOfWork;
using Temp.Domain.Models;
using Temp.Services._Helpers;
using Temp.Services.Integrations.Loggings;
using Temp.Services.Providers;
using Temp.Services.Workplaces.Models.Commands;
using Temp.Services.Workplaces.Models.Queries;

namespace Temp.Services.Workplaces;

public partial class WorkplaceService : IWorkplaceService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ILoggingBroker _loggingBroker;
    private readonly IIdentityProvider _identityProvider;

    public WorkplaceService(
        IUnitOfWork unitOfWork,
        IMapper mapper,
        ILoggingBroker loggingBroker,
        IIdentityProvider identityProvider) {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _loggingBroker = loggingBroker;
        _identityProvider = identityProvider;
    }

    public Task<CreateWorkplaceResponse> CreateWorkplace(CreateWorkplaceRequest request) =>
         TryCatch(async () => {
             var workplace = _mapper.Map<Workplace>(request);

             workplace.SetAuditableInfoOnCreate(await _identityProvider.GetCurrentUser());

             ValidateWorkplaceOnCreate(workplace);

             await _unitOfWork.Workplaces.AddAsync(workplace);
             await _unitOfWork.SaveChangesAsync();

             return _mapper.Map<CreateWorkplaceResponse>(workplace);
         });

    public Task<PagedList<GetWorkplacesResponse>> GetPagedWorkplaces(GetWorkplacesRequest request) =>
         TryCatch(async () => {
             var workplacesQuery = _unitOfWork.Workplaces
                .QueryNoTracking()
                .Where(x => x.IsActive)
                .ProjectTo<GetWorkplacesResponse>(_mapper.ConfigurationProvider);

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
            var workplaces = await _unitOfWork.Workplaces
                .QueryNoTracking()
                .Where(x => x.IsActive)
                .OrderBy(x => x.Name)
                .ProjectTo<GetWorkplaceResponse>(_mapper.ConfigurationProvider)
                .ToListAsync();

            return workplaces;
        });

    public Task<GetWorkplaceResponse> GetWorkplace(int id) =>
        TryCatch(async () => {
            var workplace = await _unitOfWork.Workplaces
                .QueryNoTracking()
                .Where(x => x.IsActive && x.Id == id)
                .ProjectTo<GetWorkplaceResponse>(_mapper.ConfigurationProvider)
                .FirstOrDefaultAsync();

            return workplace;
        });

    public Task<UpdateWorkplaceResponse> UpdateWorkplace(UpdateWorkplaceRequest request) =>
        TryCatch(async () => {
            var workplace = await _unitOfWork.Workplaces
                .FirstOrDefaultAsync(x => x.Id == request.Id);

            _mapper.Map(request, workplace);

            workplace.SetAuditableInfoOnUpdate(await _identityProvider.GetCurrentUser());

            ValidateWorkplaceOnUpdate(workplace);

            _unitOfWork.Workplaces.Update(workplace);
            await _unitOfWork.SaveChangesAsync();

            return new UpdateWorkplaceResponse();
        });

    public Task<UpdateWorkplaceStatusResponse> UpdateWorkplaceStatus(UpdateWorkplaceStatusRequest request) =>
        TryCatch(async () => {
            var workplace = await _unitOfWork.Workplaces
                .FirstOrDefaultAsync(x => x.Id == request.Id);

            workplace.IsActive = !workplace.IsActive;
            workplace.SetAuditableInfoOnUpdate(await _identityProvider.GetCurrentUser());

            ValidateWorkplaceOnStatusUpdate(workplace);

            _unitOfWork.Workplaces.Update(workplace);
            await _unitOfWork.SaveChangesAsync();

            return new UpdateWorkplaceStatusResponse();
        });


    public Task<bool> WorkplaceExists(string name) =>
        TryCatch(async () => {
            return await _unitOfWork.Workplaces.AnyAsync(x => x.Name == name);
        });


}
