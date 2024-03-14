using Microsoft.AspNetCore.Identity;
using Temp.Database;
using Temp.Domain.Models.Identity;
using Temp.Services.Employees;
using Temp.Services.Employees.Models.Commands;

namespace Temp.Services.Auth;

public class AuthService : IAuthService
{
    private readonly ApplicationDbContext _ctx;
    private readonly IConfiguration _config;
    private readonly IEmployeeService _employeeService;

    private readonly UserManager<AppUser> _userManager;
    private readonly SignInManager<AppUser> _signInManager;

    public AuthService(
        UserManager<AppUser> userManager,
        SignInManager<AppUser> signInManager,
        ApplicationDbContext ctx,
        IConfiguration config,
        IEmployeeService employeeService) {
        _userManager = userManager;
        _signInManager = signInManager;
        _ctx = ctx;
        _config = config;
        _employeeService = employeeService;
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
        var admin = await _userManager.FindByEmailAsync(request.Username);
        if (admin is null)
            return null;

        var result = await _signInManager.CheckPasswordSignInAsync(admin, request.Password, false);
        if (result.Succeeded == false)
            return null;

        var employeeId = await _ctx.Employees
            .Where(x => x.AppUserId == admin.Id)
            .Select(x => x.Id)
            .FirstOrDefaultAsync();

        return new LoginResponse {
            User = new LoginAdminResponse {
                Id = employeeId,
                Username = admin.DisplayName
            },
            Token = await GenerateToken(admin)
        };
    }

    public async Task<RegisterResponse> Register(RegisterRequest request) {
        var adminExists = await _userManager.FindByEmailAsync(request.Email) != null;

        if (adminExists) {
            return new RegisterResponse {
                Message = $"Admin already exists with {request.Email} username",
                Username = request.Email,
                Status = false
            };
        }

        var user = new AppUser {
            DisplayName = request.DisplayName,
            Email = request.Email,
            UserName = request.Email
        };

        var claims = new List<Claim>(){
           new Claim(ClaimTypes.Role, request.Role),
           new Claim(ClaimTypes.Name, user.DisplayName)
        };

        var result = await _userManager.CreateAsync(user, request.Password);
        if (result.Succeeded) {
            await _userManager.AddToRoleAsync(user, request.Role);
            await _userManager.AddClaimsAsync(user, claims);
            await _employeeService.UpdateEmployeeRole(request.Role, request.EmployeeId, user.Id);
        }

        return new RegisterResponse {
            Message = "Successful registration",
            Username = user.UserName,
            Status = result.Succeeded
        };
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
