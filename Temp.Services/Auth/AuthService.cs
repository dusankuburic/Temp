using Microsoft.AspNetCore.Identity;
using Temp.Database;
using Temp.Domain.Models.Identity;
using Temp.Services.Auth.Models.Commands;
using Temp.Services.Employees.Models.Commands;
using Temp.Services.Providers;

namespace Temp.Services.Auth;

public class AuthService : IAuthService
{
    private readonly ApplicationDbContext _ctx;
    private readonly IConfiguration _config;

    private readonly UserManager<AppUser> _userManager;
    private readonly SignInManager<AppUser> _signInManager;
    private readonly IIdentityProvider _identityProvider;

    public AuthService(
        UserManager<AppUser> userManager,
        SignInManager<AppUser> signInManager,
        ApplicationDbContext ctx,
        IConfiguration config,
        IIdentityProvider identityProvider) {
        _userManager = userManager;
        _signInManager = signInManager;
        _ctx = ctx;
        _config = config;
        _identityProvider = identityProvider;
    }

    public async Task<AssignRoleResponse> AssignRole(AssignRoleRequest request) {

        var response = await Register(new RegisterRequest {
            EmployeeId = request.Id,
            Email = request.Email,
            DisplayName = request.Username,
            Password = request.Password,
            Role = request.Role
        });

        return new AssignRoleResponse() {
            Status = response.Status
        };
    }

    public async Task<LoginResponse> Login(LoginRequest request) {
        var appUser = await _userManager.FindByEmailAsync(request.Username);
        if (appUser is null)
            return null;

        var result = await _signInManager.CheckPasswordSignInAsync(appUser, request.Password, false);
        if (result.Succeeded == false)
            return null;

        var employeeId = await _ctx.Employees
            .Where(x => x.AppUserId == appUser.Id)
            .Select(x => x.Id)
            .FirstOrDefaultAsync();

        await _identityProvider.StoreCurrentUser(appUser.Id, appUser.Email);

        return new LoginResponse {
            User = new LoginAdminResponse {
                Id = employeeId,
                Username = appUser.DisplayName
            },
            Token = await GenerateToken(appUser)
        };
    }

    public async Task<RegisterResponse> Register(RegisterRequest request) {
        var appUserExists = await _userManager.FindByEmailAsync(request.Email) != null;

        if (appUserExists) {
            return new RegisterResponse {
                Message = $"User already exists with {request.Email} username",
                Username = request.Email,
                Status = false
            };
        }

        var user = new AppUser {
            DisplayName = request.DisplayName,
            Email = request.Email,
            UserName = request.Email,
            LockoutEnd = DateTimeOffset.MaxValue
        };

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

        return new RegisterResponse {
            Message = "Successful registration",
            Username = user.UserName,
            Status = result.Succeeded
        };
    }


    public async Task<RemoveEmployeeRoleResponse> RemoveEmployeeRole(RemoveEmployeeRoleRequest request) {
        var employee = await _ctx.Employees.FirstOrDefaultAsync(x => x.Id == request.Id);

        var appUser = await _userManager.FindByIdAsync(employee.AppUserId);
        await _userManager.DeleteAsync(appUser);

        employee.AppUserId = null;
        employee.Role = "None";

        await _ctx.SaveChangesAsync();

        return new RemoveEmployeeRoleResponse();
    }


    public async Task<bool> UpdateEmployeeAccountStatus(int employeeId) {
        var employee = await _ctx.Employees
            .Where(x => x.Id == employeeId)
            .FirstOrDefaultAsync();

        var appUser = await _userManager.FindByIdAsync(employee.AppUserId);

        if (appUser.LockoutEnd != null) {
            appUser.LockoutEnd = null;
            employee.IsAppUserActive = true;
        } else {
            appUser.LockoutEnd = DateTimeOffset.MaxValue;
            employee.IsAppUserActive = false;
        }

        await _ctx.SaveChangesAsync();

        return true;
    }

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



public class LoginRequest
{
    [Required]
    [MaxLength(30)]
    public string Username { get; set; }
    [Required]
    [MaxLength(30)]
    public string Password { get; set; }
}

public class LoginAdminResponse
{
    public int Id { get; set; }
    public string Username { get; set; }
}

public class LoginResponse
{
    public string Token { get; set; }
    public LoginAdminResponse User { get; set; }
}

public class RegisterRequest
{
    public int EmployeeId { get; set; }

    public string DisplayName { get; set; }

    public string Role { get; set; }

    [Required]
    [MinLength(5), MaxLength(30)]
    public string Email { get; set; }

    [Required]
    [MinLength(5), MaxLength(30)]
    public string Password { get; set; }
}

public class RegisterResponse
{
    public string Username { get; set; }

    public string Message { get; set; }

    public bool Status { get; set; }
}
