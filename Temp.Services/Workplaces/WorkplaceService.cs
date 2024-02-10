using Temp.Core._Helpers;
using Temp.Database;
using Temp.Domain.Models;
using Temp.Services.Workplaces.Models.Command;
using Temp.Services.Workplaces.Models.Query;

namespace Temp.Services.Workplaces;

public partial class WorkplaceService : IWorkplaceService
{
    private readonly ApplicationDbContext _ctx;

    public WorkplaceService(ApplicationDbContext ctx) {
        _ctx = ctx;
    }

    public Task<CreateWorkplaceResponse> CreateWorkplace(CreateWorkplaceRequest request) =>
         TryCatch(async () => {
             var workplaceExists = await WorkplaceExists(request.Name);
             if (workplaceExists) {
                 return null;
             }

             var workplace = new Workplace {
                 Name = request.Name,
             };

             ValidateWorkplaceOnCreate(workplace);

             _ctx.Workplaces.Add(workplace);
             await _ctx.SaveChangesAsync();

             return new CreateWorkplaceResponse {
                 Id = workplace.Id,
                 Name = workplace.Name
             };
         });

    public Task<PagedList<GetWorkplacesResponse>> GetPagedWorkplaces(GetWorkplacesRequest request) =>
         TryCatch(async () => {
             var workplaces =  _ctx.Workplaces
                .Where(x => x.IsActive)
                .Select(x => new GetWorkplacesResponse {
                    Id = x.Id,
                    Name = x.Name
                }).AsQueryable();

             return await PagedList<GetWorkplacesResponse>.CreateAsync(workplaces, request.PageNumber, request.PageSize);
         });

    public Task<List<GetWorkplaceResponse>> GetWorkplaces() =>
        TryCatch(async () => {
            var workplaces = await _ctx.Workplaces
                .Where(x => x.IsActive)
                .Select(x => new GetWorkplaceResponse {
                    Id = x.Id,
                    Name = x.Name
                }).ToListAsync();

            return workplaces;
        });

    public Task<GetWorkplaceResponse> GetWorkplace(int id) =>
        TryCatch(async () => {
            var workplace = await _ctx.Workplaces
                .Where(x => x.IsActive && x.Id == id)
                .Select(x => new GetWorkplaceResponse {
                    Id = x.Id,
                    Name = x.Name
                })
                .FirstOrDefaultAsync();

            return workplace;
        });

    public Task<UpdateWorkplaceResponse> UpdateWorkplace(UpdateWorkplaceRequest request) =>
        TryCatch(async () => {
            var workpalce = await _ctx.Workplaces
                .Where(x => x.Id == request.Id)
                .FirstOrDefaultAsync();

            ValidateWorkplaceOnUpdate(workpalce);

            workpalce.Name = request.Name;

            await _ctx.SaveChangesAsync();

            return new UpdateWorkplaceResponse { Success = true };
        });

    public Task<UpdateWorkplaceStatusResponse> UpdateWorkplaceStatus(UpdateWorkplaceStatusRequest request) =>
        TryCatch(async () => {
            var workplace = await _ctx.Workplaces
                .Where(x => x.Id == request.Id)
                .FirstOrDefaultAsync();

            ValidateWorkplaceOnStatusUpdate(workplace);

            workplace.IsActive = !workplace.IsActive;

            await _ctx.SaveChangesAsync();

            return new UpdateWorkplaceStatusResponse { Success = true };
        });


    private async Task<bool> WorkplaceExists(string name) {
        return await _ctx.Workplaces.AnyAsync(x => x.Name == name);
    }
}
