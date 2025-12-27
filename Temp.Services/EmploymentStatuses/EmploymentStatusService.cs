using Temp.Database.UnitOfWork;
using Temp.Domain.Models;
using Temp.Services._Helpers;
using Temp.Services._Shared;
using Temp.Services.EmploymentStatuses.Models.Commands;
using Temp.Services.EmploymentStatuses.Models.Queries;
using Temp.Services.Integrations.Loggings;
using Temp.Services.Providers;

namespace Temp.Services.EmploymentStatuses;

public partial class EmploymentStatusService : BaseService<EmploymentStatus>, IEmploymentStatusService
{
    public EmploymentStatusService(
        IUnitOfWork unitOfWork,
        IMapper mapper,
        ILoggingBroker loggingBroker,
        IIdentityProvider identityProvider)
        : base(unitOfWork, mapper, loggingBroker, identityProvider) {
    }

    public Task<CreateEmploymentStatusResponse> CreateEmploymentStatus(CreateEmploymentStatusRequest request) =>
        TryCatch(async () => {
            var employmentStatus = Mapper.Map<EmploymentStatus>(request);

            employmentStatus.SetAuditableInfoOnCreate(await IdentityProvider.GetCurrentUser());

            ValidateEmploymentStatusOnCreate(employmentStatus);

            await UnitOfWork.EmploymentStatuses.AddAsync(employmentStatus);
            await UnitOfWork.SaveChangesAsync();

            return Mapper.Map<CreateEmploymentStatusResponse>(employmentStatus);
        });

    public Task<GetEmploymentStatusResponse> GetEmploymentStatus(GetEmploymentStatusRequest request) =>
        TryCatch(async () => {
            var employmentStatus = await UnitOfWork.EmploymentStatuses
                .QueryNoTracking()
                .Where(x => x.Id == request.Id && x.IsActive)
                .ProjectTo<GetEmploymentStatusResponse>(Mapper.ConfigurationProvider)
                .FirstOrDefaultAsync();

            return employmentStatus;
        });

    public Task<List<GetEmploymentStatusResponse>> GetEmploymentStatuses() =>
        TryCatch(async () => {
            var employmentStatuses = await UnitOfWork.EmploymentStatuses
                .QueryNoTracking()
                .Where(x => x.IsActive)
                .OrderBy(x => x.Name)
                .ProjectTo<GetEmploymentStatusResponse>(Mapper.ConfigurationProvider)
                .ToListAsync();

            return employmentStatuses;
        });

    public Task<PagedList<GetPagedEmploymentStatusesResponse>> GetPagedEmploymentStatuses(GetPagedEmploymentStatusesRequest request) =>
        TryCatch(async () => {
            var employmentStatuses = UnitOfWork.EmploymentStatuses
                .QueryNoTracking()
                .Where(x => x.IsActive)
                .ProjectTo<GetPagedEmploymentStatusesResponse>(Mapper.ConfigurationProvider);

            if (!string.IsNullOrEmpty(request.Name)) {
                employmentStatuses = employmentStatuses.Where(x => x.Name.Contains(request.Name));
            }

            employmentStatuses = employmentStatuses.OrderBy(x => x.Name);

            return await PagedList<GetPagedEmploymentStatusesResponse>.CreateAsync(
                employmentStatuses,
                request.PageNumber,
                request.PageSize);
        });

    public Task<UpdateEmploymentStatusStatusResponse> UpdateEmploymentStatusStatus(UpdateEmploymentStatusStatusRequest request) =>
        TryCatch(async () => {
            var employmentStatus = await UnitOfWork.EmploymentStatuses
                .FirstOrDefaultAsync(x => x.Id == request.Id);

            employmentStatus.IsActive = !employmentStatus.IsActive;
            employmentStatus.SetAuditableInfoOnUpdate(await IdentityProvider.GetCurrentUser());

            ValidateEmploymentStatusOnUpdate(employmentStatus);

            UnitOfWork.EmploymentStatuses.Update(employmentStatus);
            await UnitOfWork.SaveChangesAsync();

            return new UpdateEmploymentStatusStatusResponse();
        });

    public Task<UpdateEmploymentStatusResponse> UpdateEmplymentStatus(UpdateEmploymentStatusRequest request) =>
        TryCatch(async () => {
            var employmentStatus = await UnitOfWork.EmploymentStatuses
                .FirstOrDefaultAsync(x => x.Id == request.Id);

            employmentStatus.SetAuditableInfoOnUpdate(await IdentityProvider.GetCurrentUser());
            employmentStatus.Name = request.Name;

            ValidateEmploymentStatusOnUpdate(employmentStatus);

            UnitOfWork.EmploymentStatuses.Update(employmentStatus);
            await UnitOfWork.SaveChangesAsync();

            return new UpdateEmploymentStatusResponse();
        });

    public Task<bool> EmploymentStatusExists(string name) =>
        TryCatch(async () => {
            return await UnitOfWork.EmploymentStatuses.AnyAsync(x => x.Name == name);
        });
}