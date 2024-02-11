using Temp.Database;
using Temp.Domain.Models;
using Temp.Services._Helpers;
using Temp.Services.EmploymentStatuses.Models.Commands;
using Temp.Services.EmploymentStatuses.Models.Queries;

namespace Temp.Services.EmploymentStatuses;

public partial class EmploymentStatusService : IEmploymentStatusService
{
    private readonly ApplicationDbContext _ctx;

    public EmploymentStatusService(ApplicationDbContext ctx) {
        _ctx = ctx;
    }

    public Task<CreateEmploymentStatusResponse> CreateEmploymentStatus(CreateEmploymentStatusRequest request) =>
        TryCatch(async () => {
            var employmentStatus = new EmploymentStatus {
                Name = request.Name
            };

            ValidateEmploymentStatusOnCreate(employmentStatus);

            _ctx.EmploymentStatuses.Add(employmentStatus);
            await _ctx.SaveChangesAsync();

            return new CreateEmploymentStatusResponse {
                Id = employmentStatus.Id,
                Name = employmentStatus.Name
            };
        });

    public Task<GetEmploymentStatusResponse> GetEmploymentStatus(GetEmploymentStatusRequest request) =>
        TryCatch(async () => {
            var employmentStatus = await _ctx.EmploymentStatuses
                .Where(x => x.Id == request.Id && x.IsActive)
                .Select(x => new GetEmploymentStatusResponse {
                    Id = x.Id,
                    Name = x.Name
                })
                .FirstOrDefaultAsync();

            return employmentStatus;
        });

    public Task<List<GetEmploymentStatusResponse>> GetEmploymentStatuses() =>
        TryCatch(async () => {
            var employmentStatuses = await _ctx.EmploymentStatuses
                .Where(x => x.IsActive)
                .Select(x => new GetEmploymentStatusResponse {
                    Id = x.Id,
                    Name = x.Name
                })
                .ToListAsync();

            return employmentStatuses;
        });

    public Task<PagedList<GetPagedEmploymentStatusesResponse>> GetPagedEmploymentStatuses(GetPagedEmploymentStatusesRequest request) =>
        TryCatch(async () => {
            var pagedEmploymentStatuses = _ctx.EmploymentStatuses
                .Where(x => x.IsActive)
                .Select(x => new GetPagedEmploymentStatusesResponse {
                    Id = x.Id,
                    Name = x.Name
                }).AsQueryable();

            return await PagedList<GetPagedEmploymentStatusesResponse>.CreateAsync(pagedEmploymentStatuses, request.PageNumber, request.PageSize);
        });

    public Task<UpdateEmploymentStatusStatusResponse> UpdateEmploymentStatusStatus(UpdateEmploymentStatusStatusRequest request) =>
        TryCatch(async () => {
            var employmentStatus = await _ctx.EmploymentStatuses
                .Where(x => x.Id == request.Id)
                .FirstOrDefaultAsync();

            ValidateEmploymentStatusOnUpdate(employmentStatus);

            employmentStatus.IsActive = !employmentStatus.IsActive;

            await _ctx.SaveChangesAsync();

            return new UpdateEmploymentStatusStatusResponse {
                Success = true
            };
        });

    public Task<UpdateEmploymentStatusResponse> UpdateEmplymentStatus(UpdateEmploymentStatusRequest request) =>
        TryCatch(async () => {
            var employmentStatus = await _ctx.EmploymentStatuses
                .Where(x => x.Id == request.Id)
                .FirstOrDefaultAsync();

            ValidateEmploymentStatusOnUpdate(employmentStatus);

            employmentStatus.Name = request.Name;

            await _ctx.SaveChangesAsync();

            return new UpdateEmploymentStatusResponse {
                Success = true
            };
        });
}
