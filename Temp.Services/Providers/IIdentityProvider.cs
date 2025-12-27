using Temp.Services.Providers.Models;

namespace Temp.Services.Providers;

public interface IIdentityProvider
{
    Task<CurrentUser> GetCurrentUser();
    Task<bool> RemoveCurrentUser();
    Task<bool> StoreCurrentUser(string appUserId, string email);
}