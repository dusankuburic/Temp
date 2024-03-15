using Temp.Database;
using Temp.Domain.Models;
using Temp.Services._Helpers;
using Temp.Services.EmploymentStatuses.Models.Commands;
using Temp.Services.EmploymentStatuses.Models.Queries;
using Temp.Services.Integrations.Loggings;
using Temp.Services.Providers;

namespace Temp.Services.EmploymentStatuses;

public partial class EmploymentStatusService : IEmploymentStatusService
{
    private readonly ApplicationDbContext _ctx;
    private readonly IMapper _mapper;
    private readonly ILoggingBroker _loggingBroker;
    private readonly IIdentityProvider _identityProvider;

    public EmploymentStatusService(
        ApplicationDbContext ctx,
        IMapper mapper,
        ILoggingBroker loggingBroker,
        IIdentityProvider identityProvider) {
        _ctx = ctx;
        _mapper = mapper;
        _loggingBroker = loggingBroker;
        _identityProvider = identityProvider;
    }

    public Task<CreateEmploymentStatusResponse> CreateEmploymentStatus(CreateEmploymentStatusRequest request) =>
        TryCatch(async () => {
            var employmentStatus = _mapper.Map<EmploymentStatus>(request);

            employmentStatus.SetAuditableInfoOnCreate(await _identityProvider.GetCurrentUser());

            ValidateEmploymentStatusOnCreate(employmentStatus);

            _ctx.EmploymentStatuses.Add(employmentStatus);
            await _ctx.SaveChangesAsync();

            return _mapper.Map<CreateEmploymentStatusResponse>(employmentStatus);
        });

    public Task<GetEmploymentStatusResponse> GetEmploymentStatus(GetEmploymentStatusRequest request) =>
        TryCatch(async () => {
            var employmentStatus = await _ctx.EmploymentStatuses
                .Where(x => x.Id == request.Id && x.IsActive)
                .ProjectTo<GetEmploymentStatusResponse>(_mapper.ConfigurationProvider)
                .FirstOrDefaultAsync();

            return employmentStatus;
        });

    public Task<List<GetEmploymentStatusResponse>> GetEmploymentStatuses() =>
        TryCatch(async () => {
            var employmentStatuses = await _ctx.EmploymentStatuses
                .Where(x => x.IsActive)
                .ProjectTo<GetEmploymentStatusResponse>(_mapper.ConfigurationProvider)
                .ToListAsync();

            return employmentStatuses;
        });

    public Task<PagedList<GetPagedEmploymentStatusesResponse>> GetPagedEmploymentStatuses(GetPagedEmploymentStatusesRequest request) =>
        TryCatch(async () => {
            var employmentStatuses = _ctx.EmploymentStatuses
                .Where(x => x.IsActive)
                .ProjectTo<GetPagedEmploymentStatusesResponse>(_mapper.ConfigurationProvider)
                .AsQueryable();

            if (!string.IsNullOrEmpty(request.Name)) {
                employmentStatuses = employmentStatuses.Where(x => x.Name.Contains(request.Name))
                    .AsQueryable();
            }

            return await PagedList<GetPagedEmploymentStatusesResponse>.CreateAsync(
                employmentStatuses,
                request.PageNumber,
                request.PageSize);
        });

    public Task<UpdateEmploymentStatusStatusResponse> UpdateEmploymentStatusStatus(UpdateEmploymentStatusStatusRequest request) =>
        TryCatch(async () => {
            var employmentStatus = await _ctx.EmploymentStatuses
                .Where(x => x.Id == request.Id)
                .FirstOrDefaultAsync();

            employmentStatus.IsActive = !employmentStatus.IsActive;
            employmentStatus.SetAuditableInfoOnUpdate(await _identityProvider.GetCurrentUser());

            ValidateEmploymentStatusOnUpdate(employmentStatus);

            await _ctx.SaveChangesAsync();

            return new UpdateEmploymentStatusStatusResponse();
        });

    public Task<UpdateEmploymentStatusResponse> UpdateEmplymentStatus(UpdateEmploymentStatusRequest request) =>
        TryCatch(async () => {
            var employmentStatus = await _ctx.EmploymentStatuses
                .Where(x => x.Id == request.Id)
                .FirstOrDefaultAsync();

            employmentStatus.SetAuditableInfoOnUpdate(await _identityProvider.GetCurrentUser());
            employmentStatus.Name = request.Name;

            ValidateEmploymentStatusOnUpdate(employmentStatus);

            await _ctx.SaveChangesAsync();

            return new UpdateEmploymentStatusResponse();
        });

    public Task<bool> EmploymentStatusExists(string name) =>
        TryCatch(async () => {
            return await _ctx.EmploymentStatuses.AnyAsync(x => x.Name == name);
        });
}
