using AutoMapper.QueryableExtensions;
using Temp.Database;
using Temp.Domain.Models;
using Temp.Services._Helpers;
using Temp.Services.Workplaces.Models.Commands;
using Temp.Services.Workplaces.Models.Queries;

namespace Temp.Services.Workplaces;

public partial class WorkplaceService : IWorkplaceService
{
    private readonly ApplicationDbContext _ctx;
    private readonly IMapper _mapper;

    public WorkplaceService(ApplicationDbContext ctx, IMapper mapper) {
        _ctx = ctx;
        _mapper = mapper;
    }

    public Task<CreateWorkplaceResponse> CreateWorkplace(CreateWorkplaceRequest request) =>
         TryCatch(async () => {
             var workplace = _mapper.Map<Workplace>(request);

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

             return await PagedList<GetWorkplacesResponse>.CreateAsync(
                 workplacesQuery,
                 request.PageNumber,
                 request.PageSize);
         });

    public Task<List<GetWorkplaceResponse>> GetWorkplaces() =>
        TryCatch(async () => {
            var workplaces = await _ctx.Workplaces
                .Where(x => x.IsActive)
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
            var workpalce = await _ctx.Workplaces
                .Where(x => x.Id == request.Id)
                .FirstOrDefaultAsync();

            _mapper.Map(request, workpalce);

            ValidateWorkplaceOnUpdate(workpalce);

            await _ctx.SaveChangesAsync();

            return new UpdateWorkplaceResponse();
        });

    public Task<UpdateWorkplaceStatusResponse> UpdateWorkplaceStatus(UpdateWorkplaceStatusRequest request) =>
        TryCatch(async () => {
            var workplace = await _ctx.Workplaces
                .Where(x => x.Id == request.Id)
                .FirstOrDefaultAsync();

            workplace.IsActive = !workplace.IsActive;

            ValidateWorkplaceOnStatusUpdate(workplace);

            await _ctx.SaveChangesAsync();

            return new UpdateWorkplaceStatusResponse();
        });


    private async Task<bool> WorkplaceExists(string name) {
        return await _ctx.Workplaces.AnyAsync(x => x.Name == name);
    }
}
