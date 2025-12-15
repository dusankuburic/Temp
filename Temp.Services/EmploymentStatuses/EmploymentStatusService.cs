using Temp.Database.UnitOfWork;
using Temp.Domain.Models;
using Temp.Services._Helpers;
using Temp.Services.EmploymentStatuses.Models.Commands;
using Temp.Services.EmploymentStatuses.Models.Queries;
using Temp.Services.Integrations.Loggings;
using Temp.Services.Providers;

namespace Temp.Services.EmploymentStatuses;

public partial class EmploymentStatusService : IEmploymentStatusService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ILoggingBroker _loggingBroker;
    private readonly IIdentityProvider _identityProvider;

    public EmploymentStatusService(
        IUnitOfWork unitOfWork,
        IMapper mapper,
        ILoggingBroker loggingBroker,
        IIdentityProvider identityProvider) {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _loggingBroker = loggingBroker;
        _identityProvider = identityProvider;
    }

    public Task<CreateEmploymentStatusResponse> CreateEmploymentStatus(CreateEmploymentStatusRequest request) =>
        TryCatch(async () => {
            var employmentStatus = _mapper.Map<EmploymentStatus>(request);

            employmentStatus.SetAuditableInfoOnCreate(await _identityProvider.GetCurrentUser());

            ValidateEmploymentStatusOnCreate(employmentStatus);

            await _unitOfWork.EmploymentStatuses.AddAsync(employmentStatus);
            await _unitOfWork.SaveChangesAsync();

            return _mapper.Map<CreateEmploymentStatusResponse>(employmentStatus);
        });

    public Task<GetEmploymentStatusResponse> GetEmploymentStatus(GetEmploymentStatusRequest request) =>
        TryCatch(async () => {
            var employmentStatus = await _unitOfWork.EmploymentStatuses
                .QueryNoTracking()
                .Where(x => x.Id == request.Id && x.IsActive)
                .ProjectTo<GetEmploymentStatusResponse>(_mapper.ConfigurationProvider)
                .FirstOrDefaultAsync();

            return employmentStatus;
        });

    public Task<List<GetEmploymentStatusResponse>> GetEmploymentStatuses() =>
        TryCatch(async () => {
            var employmentStatuses = await _unitOfWork.EmploymentStatuses
                .QueryNoTracking()
                .Where(x => x.IsActive)
                .OrderBy(x => x.Name)
                .ProjectTo<GetEmploymentStatusResponse>(_mapper.ConfigurationProvider)
                .ToListAsync();

            return employmentStatuses;
        });

    public Task<PagedList<GetPagedEmploymentStatusesResponse>> GetPagedEmploymentStatuses(GetPagedEmploymentStatusesRequest request) =>
        TryCatch(async () => {
            var employmentStatuses = _unitOfWork.EmploymentStatuses
                .QueryNoTracking()
                .Where(x => x.IsActive)
                .ProjectTo<GetPagedEmploymentStatusesResponse>(_mapper.ConfigurationProvider);

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
            var employmentStatus = await _unitOfWork.EmploymentStatuses
                .FirstOrDefaultAsync(x => x.Id == request.Id);

            employmentStatus.IsActive = !employmentStatus.IsActive;
            employmentStatus.SetAuditableInfoOnUpdate(await _identityProvider.GetCurrentUser());

            ValidateEmploymentStatusOnUpdate(employmentStatus);

            _unitOfWork.EmploymentStatuses.Update(employmentStatus);
            await _unitOfWork.SaveChangesAsync();

            return new UpdateEmploymentStatusStatusResponse();
        });

    public Task<UpdateEmploymentStatusResponse> UpdateEmplymentStatus(UpdateEmploymentStatusRequest request) =>
        TryCatch(async () => {
            var employmentStatus = await _unitOfWork.EmploymentStatuses
                .FirstOrDefaultAsync(x => x.Id == request.Id);

            employmentStatus.SetAuditableInfoOnUpdate(await _identityProvider.GetCurrentUser());
            employmentStatus.Name = request.Name;

            ValidateEmploymentStatusOnUpdate(employmentStatus);

            _unitOfWork.EmploymentStatuses.Update(employmentStatus);
            await _unitOfWork.SaveChangesAsync();

            return new UpdateEmploymentStatusResponse();
        });

    public Task<bool> EmploymentStatusExists(string name) =>
        TryCatch(async () => {
            return await _unitOfWork.EmploymentStatuses.AnyAsync(x => x.Name == name);
        });
}
