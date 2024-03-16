using Temp.Database;
using Temp.Domain.Models;
using Temp.Services._Helpers;
using Temp.Services.Integrations.Loggings;
using Temp.Services.Providers;
using Temp.Services.Workplaces.Models.Commands;
using Temp.Services.Workplaces.Models.Queries;

namespace Temp.Services.Workplaces;

public partial class WorkplaceService : IWorkplaceService
{
    private readonly ApplicationDbContext _ctx;
    private readonly IMapper _mapper;
    private readonly ILoggingBroker _loggingBroker;
    private readonly IIdentityProvider _identityProvider;

    public WorkplaceService(
        ApplicationDbContext ctx,
        IMapper mapper,
        ILoggingBroker loggingBroker,
        IIdentityProvider identityProvider) {
        _ctx = ctx;
        _mapper = mapper;
        _loggingBroker = loggingBroker;
        _identityProvider = identityProvider;
    }

    public Task<CreateWorkplaceResponse> CreateWorkplace(CreateWorkplaceRequest request) =>
         TryCatch(async () => {
             var workplace = _mapper.Map<Workplace>(request);

             workplace.SetAuditableInfoOnCreate(await _identityProvider.GetCurrentUser());

             ValidateWorkplaceOnCreate(workplace);

             _ctx.Workplaces.Add(workplace);
             await _ctx.SaveChangesAsync();

             return _mapper.Map<CreateWorkplaceResponse>(workplace);
         });

    public Task<PagedList<GetWorkplacesResponse>> GetPagedWorkplaces(GetWorkplacesRequest request) =>
         TryCatch(async () => {
             var workplacesQuery =  _ctx.Workplaces
                .Where(x => x.IsActive)
                .ProjectTo<GetWorkplacesResponse>(_mapper.ConfigurationProvider)
                .AsQueryable();

             if (!string.IsNullOrEmpty(request.Name)) {
                 workplacesQuery = workplacesQuery.Where(x => x.Name.Contains(request.Name))
                    .AsQueryable();
             }

             workplacesQuery = workplacesQuery.OrderBy(x => x.Name)
                .AsQueryable();

             return await PagedList<GetWorkplacesResponse>.CreateAsync(
                 workplacesQuery,
                 request.PageNumber,
                 request.PageSize);
         });

    public Task<List<GetWorkplaceResponse>> GetWorkplaces() =>
        TryCatch(async () => {
            var workplaces = await _ctx.Workplaces
                .Where(x => x.IsActive)
                .OrderBy(x => x.Name)
                .ProjectTo<GetWorkplaceResponse>(_mapper.ConfigurationProvider)
                .ToListAsync();

            return workplaces;
        });

    public Task<GetWorkplaceResponse> GetWorkplace(int id) =>
        TryCatch(async () => {
            var workplace = await _ctx.Workplaces
                .Where(x => x.IsActive && x.Id == id)
                .ProjectTo<GetWorkplaceResponse>(_mapper.ConfigurationProvider)
                .FirstOrDefaultAsync();

            return workplace;
        });

    public Task<UpdateWorkplaceResponse> UpdateWorkplace(UpdateWorkplaceRequest request) =>
        TryCatch(async () => {
            var workplace = await _ctx.Workplaces
                .Where(x => x.Id == request.Id)
                .FirstOrDefaultAsync();

            _mapper.Map(request, workplace);

            workplace.SetAuditableInfoOnUpdate(await _identityProvider.GetCurrentUser());

            ValidateWorkplaceOnUpdate(workplace);

            await _ctx.SaveChangesAsync();

            return new UpdateWorkplaceResponse();
        });

    public Task<UpdateWorkplaceStatusResponse> UpdateWorkplaceStatus(UpdateWorkplaceStatusRequest request) =>
        TryCatch(async () => {
            var workplace = await _ctx.Workplaces
                .Where(x => x.Id == request.Id)
                .FirstOrDefaultAsync();

            workplace.IsActive = !workplace.IsActive;
            workplace.SetAuditableInfoOnUpdate(await _identityProvider.GetCurrentUser());

            ValidateWorkplaceOnStatusUpdate(workplace);

            await _ctx.SaveChangesAsync();

            return new UpdateWorkplaceStatusResponse();
        });


    public Task<bool> WorkplaceExists(string name) =>
        TryCatch(async () => {
            return await _ctx.Workplaces.AnyAsync(x => x.Name == name);
        });


}
