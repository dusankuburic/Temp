using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using StackExchange.Redis;
using Temp.Domain.Models.Identity;
using Temp.Services.Providers.Models;

namespace Temp.Services.Providers;

public class IdentityProvider : IIdentityProvider
{
    private readonly string _currentUserIdPrefix = "curr_user_";
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IDatabase _database;
    private readonly UserManager<AppUser> _userManager;

    public IdentityProvider(
        IHttpContextAccessor httpContextAccessor,
        IConnectionMultiplexer redis,
        UserManager<AppUser> userManager) {
        _httpContextAccessor = httpContextAccessor;
        _database = redis.GetDatabase();
        _userManager = userManager;
    }

    public async Task<bool> StoreCurrentUser(string appUserId, string email) {
        var currentUser = new CurrentUser {
            AppUserId = appUserId,
            Email = email
        };

        var stored = await _database.StringSetAsync(
            $"{_currentUserIdPrefix}{email}",
            JsonConvert.SerializeObject(currentUser),
            TimeSpan.FromHours(2));

        return stored ? true : false;
    }

    public async Task<CurrentUser> GetCurrentUser() {

        var currentUserEmail = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.Email);
        var data = await _database.StringGetAsync($"{_currentUserIdPrefix}{currentUserEmail}");

        if (data.IsNullOrEmpty) {
            var appUserId = await _userManager.Users
                .Where(x => x.Email == currentUserEmail)
                .Select(x => x.Id)
                .FirstOrDefaultAsync();

            var currentUser = new CurrentUser {
                Email = currentUserEmail,
                AppUserId = appUserId
            };

            var stored = await _database.StringSetAsync(
                $"{_currentUserIdPrefix}{currentUserEmail}",
                JsonConvert.SerializeObject(currentUser),
                TimeSpan.FromHours(2));

            return currentUser;
        }

        return JsonConvert.DeserializeObject<CurrentUser>(data);
    }

    public async Task<bool> RemoveCurrentUser() {
        var currentUserEmail = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.Email);
        await _database.KeyDeleteAsync($"{_currentUserIdPrefix}{currentUserEmail}");

        return true;
    }
}
