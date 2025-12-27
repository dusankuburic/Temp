using Temp.Database.UnitOfWork;
using Temp.Services.Integrations.Loggings;
using Temp.Services.Providers;
using Temp.Services.Providers.Models;

namespace Temp.Services._Shared;

public abstract class BaseService
{
    protected readonly IUnitOfWork UnitOfWork;
    protected readonly IMapper? Mapper;
    protected readonly ILoggingBroker Logger;
    protected readonly IIdentityProvider IdentityProvider;

    protected BaseService(
        IUnitOfWork unitOfWork,
        IMapper? mapper,
        ILoggingBroker loggingBroker,
        IIdentityProvider identityProvider) {
        UnitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        Mapper = mapper;
        Logger = loggingBroker ?? throw new ArgumentNullException(nameof(loggingBroker));
        IdentityProvider = identityProvider ?? throw new ArgumentNullException(nameof(identityProvider));
    }

    protected Task<CurrentUser> GetCurrentUserAsync() => IdentityProvider.GetCurrentUser();

    protected async Task<string> GetCurrentUserIdAsync() {
        var user = await GetCurrentUserAsync();
        return user.AppUserId;
    }

    protected void LogInfo(string message) => Logger.LogInformation(message);

    protected void LogWarning(string message) => Logger.LogWarning(message);

    protected void LogError(Exception exception) => Logger.LogError(exception);

    protected void LogCritical(Exception exception) => Logger.LogCritical(exception);

    protected void LogDebug(string message) => Logger.LogDebug(message);

    protected Task<int> SaveChangesAsync(CancellationToken cancellationToken = default) =>
        UnitOfWork.SaveChangesAsync(cancellationToken);

    protected TDestination Map<TDestination>(object source) => Mapper.Map<TDestination>(source);

    protected TDestination Map<TSource, TDestination>(TSource source, TDestination destination) =>
        Mapper.Map(source, destination);
}

public abstract class BaseService<TEntity> : BaseService where TEntity : class
{
    protected BaseService(
        IUnitOfWork unitOfWork,
        IMapper? mapper,
        ILoggingBroker loggingBroker,
        IIdentityProvider identityProvider)
        : base(unitOfWork, mapper, loggingBroker, identityProvider) {
    }
}