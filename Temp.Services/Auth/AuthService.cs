using Temp.Database;
using Temp.Domain.Models.Identity;
using Temp.Services.Auth.Models.Commands;
using Temp.Services.Integrations.Loggings;
using Temp.Services.Providers;

namespace Temp.Services.Auth;

public partial class AuthService : IAuthService
{
    private readonly ApplicationDbContext _ctx;
    private readonly IConfiguration _config;

    private readonly UserManager<AppUser> _userManager;
    private readonly SignInManager<AppUser> _signInManager;
    private readonly ILoggingBroker _loggingBroker;
    private readonly IIdentityProvider _identityProvider;

    public AuthService(
        UserManager<AppUser> userManager,
        SignInManager<AppUser> signInManager,
        ApplicationDbContext ctx,
        IConfiguration config,
        ILoggingBroker loggingBroker,
        IIdentityProvider identityProvider) {
        _userManager = userManager;
        _signInManager = signInManager;
        _ctx = ctx;
        _config = config;
        _loggingBroker = loggingBroker;
        _identityProvider = identityProvider;
    }

    public Task<LoginAppUserResponse> Login(LoginAppUserRequest request) =>
        TryCatch(async () => {
            var appUser = await _userManager.FindByEmailAsync(request.Username);
            ValidateUserIsNull(appUser);

            var result = await _signInManager.CheckPasswordSignInAsync(appUser, request.Password, false);
            ValidateOnLogin(result);

            var employeeId = await _ctx.Employees
            .Where(x => x.AppUserId == appUser.Id)
            .Select(x => x.Id)
            .FirstOrDefaultAsync();

            await _identityProvider.StoreCurrentUser(appUser.Id, appUser.Email);

            return new LoginAppUserResponse {
                User = new LoginAppUser {
                    Id = employeeId,
                    Username = appUser.DisplayName
                },
                Token = await GenerateToken(appUser)
            };
        });

    public async Task<bool> Logout() {
        await _identityProvider.RemoveCurrentUser();

        return true;
    }

    public async Task<bool> CheckUsernameExists(string username) {
        return await _userManager.FindByEmailAsync(username) != null;
    }


    public Task<AppUser> Register(RegisterAppUserRequest request) =>
        TryCatch(async () => {
            var user = new AppUser {
                DisplayName = request.DisplayName,
                Email = request.Email,
                UserName = request.Email,
                LockoutEnd = DateTimeOffset.MaxValue
            };

            ValidateUserOnCreate(user);

            var claims = new List<Claim>(){
               new Claim(ClaimTypes.Role, request.Role),
               new Claim(ClaimTypes.Name, user.DisplayName),
               new Claim(ClaimTypes.Email, user.Email)
            };

            var result = await _userManager.CreateAsync(user, request.Password);
            if (result.Succeeded) {
                await _userManager.AddToRoleAsync(user, request.Role);
                await _userManager.AddClaimsAsync(user, claims);
                await UpdateEmployeeRole(request.Role, request.EmployeeId, user.Id);
            }

            return user;
        });

    public Task<AppUser> RemoveEmployeeRole(RemoveEmployeeRoleRequest request) =>
        TryCatch(async () => {
            var employee = await _ctx.Employees.FirstOrDefaultAsync(x => x.Id == request.Id);

            var appUser = await _userManager.FindByIdAsync(employee.AppUserId);
            ValidateUserIdIsNull(appUser.Id);

            await _userManager.DeleteAsync(appUser);

            employee.AppUserId = null;
            employee.Role = "None";

            await _ctx.SaveChangesAsync();

            return appUser;
        });

    public Task<AppUser> UpdateEmployeeAccountStatus(int employeeId) =>
        TryCatch(async () => {
            var employee = await _ctx.Employees
            .Where(x => x.Id == employeeId)
            .FirstOrDefaultAsync();
            ValidateUserIdIsNull(employee.AppUserId);

            var appUser = await _userManager.FindByIdAsync(employee.AppUserId);
            ValidateUserIsNull(appUser);

            if (appUser.LockoutEnd != null) {
                appUser.LockoutEnd = null;
                employee.IsAppUserActive = true;
            } else {
                appUser.LockoutEnd = DateTimeOffset.MaxValue;
                employee.IsAppUserActive = false;
            }

            await _ctx.SaveChangesAsync();

            return appUser;
        });

    private async Task<bool> UpdateEmployeeRole(string RoleName, int EmployeeId, string appUserId) {
        var empolyee = await _ctx.Employees
            .Where(x => x.Id == EmployeeId)
            .FirstOrDefaultAsync();

        empolyee.Role = RoleName;
        empolyee.AppUserId = appUserId;
        empolyee.IsAppUserActive = false;
        await _ctx.SaveChangesAsync();

        return empolyee.Role == RoleName;
    }

    private async Task<string> GenerateToken(AppUser appUser) {
        var appUserClaims = await _userManager.GetClaimsAsync(appUser);

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config.GetSection("AppSettings:Token").Value));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(appUserClaims),
            Expires = DateTime.UtcNow.AddDays(1),
            SigningCredentials = creds,
            Issuer = _config["AppSettings:Issuer"]
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.CreateToken(tokenDescriptor);

        return tokenHandler.WriteToken(token);
    }
}