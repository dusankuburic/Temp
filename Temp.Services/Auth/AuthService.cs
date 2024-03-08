using Temp.Database;
using Temp.Domain.Models;
using Temp.Services.Employees;
using Temp.Services.Employees.Models.Commands;

namespace Temp.Services.Auth;

[Obsolete("REMINDER(FIRST AID): Whole auth should be rewritten with existing libs, db relations updated, cache creds, due to time constraints leave it alone for now...")]
public class AuthService : IAuthService
{
    private readonly ApplicationDbContext _ctx;
    private readonly IConfiguration _config;
    private readonly IEmployeeService _employeeService;

    public AuthService(
        ApplicationDbContext ctx,
        IConfiguration config,
        IEmployeeService employeeService) {
        _ctx = ctx;
        _config = config;
        _employeeService = employeeService;
    }

    public async Task<AssignRoleResponse> AssignRole(AssignRoleRequest request) {
        if (request.Role == "User") {
            var userRequest = new RegisterUserRequest
                {
                Username = request.Username,
                Password = request.Password,
                EmployeeId = request.Id
            };

            var response = await RegisterUser(userRequest);

            return new AssignRoleResponse {
                Username = response.Username,
                Message = response.Messsage,
                Status = response.Status
            };

        } else if (request.Role == "Admin") {
            var adminRequest = new RegisterAdminRequest
                {
                Username = request.Username,
                Password = request.Password,
                EmployeeId = request.Id
            };

            var response = await RegisterAdmin(adminRequest);

            return new AssignRoleResponse {
                Username = response.Username,
                Message = response.Message,
                Status = response.Status
            };
        } else if (request.Role == "Moderator") {
            var moderatorRequest = new RegisterModeratorRequest
                {
                Username = request.Username,
                Password = request.Password,
                EmployeeId = request.Id
            };

            var response = await RegisterModerator(moderatorRequest);

            return new AssignRoleResponse {
                Username = response.Username,
                Message = response.Message,
                Status = response.Status
            };
        } else {
            return new AssignRoleResponse {
                Status = false,
                Message = "Wrong role!!!!"
            };
        }
    }

    public async Task<LoginModeratorResponse> LoginModerator(LoginModeratorRequest request) {
        var moderator = await _ctx.Moderators.FirstOrDefaultAsync(x => x.Username == request.Username);

        if (moderator is null)
            return null;

        if (moderator.IsActive == false)
            return null;

        if (!VerifyPasswordHash(request.Password, moderator.PasswordHash, moderator.PasswordSalt))
            return null;

        var moderatorClaims = new List<Claim>()
            {
                new Claim(ClaimTypes.Name, moderator.Username),
                new Claim(ClaimTypes.Role, "Moderator")
            };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config.GetSection("AppSettings:Token").Value));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);
        var moderatorIdentity = new ClaimsIdentity(moderatorClaims, "Moderator_Identity");

        var tokenDescriptor = new SecurityTokenDescriptor
            {
            Subject = moderatorIdentity,
            Expires = DateTime.UtcNow.AddDays(1),
            SigningCredentials = creds,
            Issuer = _config["AppSettings:Issuer"]
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.CreateToken(tokenDescriptor);

        return new LoginModeratorResponse {
            User = new ModeratorResponse {
                Id = moderator.Id,
                Username = moderator.Username
            },
            Token = tokenHandler.WriteToken(token)
        };
    }

    public async Task<RegisterModeratorResponse> RegisterModerator(RegisterModeratorRequest request) {
        var moderatorExists = await ModeratorExists(request.Username);

        if (moderatorExists) {
            return new RegisterModeratorResponse {
                Message = $"Admin already exists with {request.Username} username",
                Username = request.Username,
                Status = false
            };
        }

        byte[] passwordHash, passwordSalt;
        CreatePasswordHash(request.Password, out passwordHash, out passwordSalt);

        var moderator = new Moderator
            {
            Username = request.Username,
            PasswordHash = passwordHash,
            PasswordSalt = passwordSalt,
            EmployeeId = request.EmployeeId
        };

        _ctx.Moderators.Add(moderator);
        await _ctx.SaveChangesAsync();

        await _employeeService.UpdateEmployeeRole("Moderator", request.EmployeeId);

        return new RegisterModeratorResponse {
            Message = "Successful registration",
            Username = moderator.Username,
            Status = true
        };
    }

    private async Task<bool> ModeratorExists(string username) {
        return await _ctx.Moderators.AnyAsync(x => x.Username == username);
    }

    public async Task<Response> LoginUser(LoginUserRequest request) {
        var user = await _ctx.Users.FirstOrDefaultAsync(x => x.Username == request.Username);

        if (user is null)
            return null;

        if (user.IsActive == false)
            return null;


        if (!VerifyPasswordHash(request.Password, user.PasswordHash, user.PasswordSalt))
            return null;

        var userClaims = new List<Claim>()
            {
                new Claim(ClaimTypes.Name,  user.Username),
                new Claim(ClaimTypes.Role, "User")
            };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config.GetSection("AppSettings:Token").Value));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);
        var userIdentity = new ClaimsIdentity(userClaims, "User_Identity");

        var tokenDescriptor = new SecurityTokenDescriptor
            {
            Subject = userIdentity,
            Expires = DateTime.UtcNow.AddDays(1),
            SigningCredentials = creds,
            Issuer = _config["AppSettings:Issuer"]
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.CreateToken(tokenDescriptor);

        return new Response {
            User = new UserResponse {
                Id = user.Id,
                Username = user.Username
            },
            Token = tokenHandler.WriteToken(token)
        };
    }

    public async Task<RegisterUserResponse> RegisterUser(RegisterUserRequest request) {
        var userExists = await UserExists(request.Username);

        if (userExists) {
            return new RegisterUserResponse {
                Messsage = $"User already exists with {request.Username} username",
                Username = request.Username,
                Status = false
            };
        }

        byte[] passwordHash, passwordSalt;
        CreatePasswordHash(request.Password, out passwordHash, out passwordSalt);

        var user = new User {
            Username = request.Username,
            PasswordHash = passwordHash,
            PasswordSalt = passwordSalt,
            EmployeeId = request.EmployeeId
        };

        _ctx.Users.Add(user);
        await _ctx.SaveChangesAsync();

        await _employeeService.UpdateEmployeeRole("User", request.EmployeeId);

        return new RegisterUserResponse {
            Messsage = "Successful registration",
            Username = user.Username,
            Status = true
        };
    }

    private async Task<bool> UserExists(string username) {
        return await _ctx.Users.AnyAsync(x => x.Username == username);
    }

    public async Task<LoginAResponse> LoginAdmin(LoginAdminRequest request) {
        var admin = await _ctx.Admins.FirstOrDefaultAsync(x => x.Username == request.Username);

        if (admin is null)
            return null;

        if (admin.IsActive == false)
            return null;

        if (!VerifyPasswordHash(request.Password, admin.PasswordHash, admin.PasswordSalt))
            return null;


        var adminClaims = new List<Claim>()
        {
            new Claim(ClaimTypes.Name, admin.Username),
            new Claim(ClaimTypes.Role, "Admin")
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config.GetSection("AppSettings:Token").Value));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);
        var adminIdentity = new ClaimsIdentity(adminClaims, "Admin_Identity");

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = adminIdentity,
            Expires = DateTime.UtcNow.AddDays(1),
            SigningCredentials = creds,
            Issuer = _config["AppSettings:Issuer"]
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.CreateToken(tokenDescriptor);

        return new LoginAResponse {
            User = new LoginAdminResponse {
                Id = admin.Id,
                Username = admin.Username
            },
            Token = tokenHandler.WriteToken(token)
        };
    }

    public async Task<RegisterAdminResponse> RegisterAdmin(RegisterAdminRequest request) {
        var adminExists = await AdminExists(request.Username);

        if (adminExists) {
            return new RegisterAdminResponse {
                Message = $"Admin already exists with {request.Username} username",
                Username = request.Username,
                Status = false
            };
        }

        byte[] passwordHash, passwordSalt;
        CreatePasswordHash(request.Password, out passwordHash, out passwordSalt);

        var admin = new Admin
        {
            Username = request.Username,
            PasswordHash = passwordHash,
            PasswordSalt = passwordSalt,
            EmployeeId = request.EmployeeId
        };

        _ctx.Admins.Add(admin);
        await _ctx.SaveChangesAsync();

        await _employeeService.UpdateEmployeeRole("Admin", request.EmployeeId);

        return new RegisterAdminResponse {
            Message = "Successful registration",
            Username = admin.Username,
            Status = true
        };
    }

    private async Task<bool> AdminExists(string username) {
        return await _ctx.Admins.AnyAsync(x => x.Username == username);
    }


    private bool VerifyPasswordHash(string password, byte[] passwordHash, byte[] passwordSalt) {
        using (var hmac = new HMACSHA512(passwordSalt)) {
            var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
            for (int i = 0; i < computedHash.Length; i++) {
                if (computedHash[i] != passwordHash[i])
                    return false;
            }
        }
        return true;
    }

    private void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt) {
        using (var hmac = new HMACSHA512()) {
            passwordSalt = hmac.Key;
            passwordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
        }
    }
}



public class LoginAdminRequest
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

public class LoginAResponse
{
    public string Token { get; set; }
    public LoginAdminResponse User { get; set; }
}

public class RegisterAdminRequest
{
    public int EmployeeId { get; set; }

    [Required]
    [MinLength(5), MaxLength(30)]
    public string Username { get; set; }

    [Required]
    [MinLength(5), MaxLength(30)]
    public string Password { get; set; }
}

public class RegisterAdminResponse
{
    public string Username { get; set; }

    public string Message { get; set; }

    public bool Status { get; set; }
}


//USER
public class LoginUserRequest
{
    [Required]
    public string Username { get; set; }
    [Required]
    public string Password { get; set; }
}

public class UserResponse
{
    public int Id { get; set; }
    public string Username { get; set; }
}

public class Response
{
    public string Token { get; set; }
    public UserResponse User { get; set; }
}


public class RegisterUserRequest
{
    public int EmployeeId { get; set; }
    [Required]
    public string Username { get; set; }
    [Required]
    public string Password { get; set; }
}

public class RegisterUserResponse
{
    public string Username { get; set; }
    public string Messsage { get; set; }
    public bool Status { get; set; }
}

//MODERATOR
public class LoginModeratorRequest
{
    [Required]
    [MaxLength(30)]
    public string Username { get; set; }

    [Required]
    [MaxLength(30)]
    public string Password { get; set; }
}

public class ModeratorResponse
{
    public int Id { get; set; }
    public string Username { get; set; }
}

public class LoginModeratorResponse
{
    public string Token { get; set; }
    public ModeratorResponse User { get; set; }
}


public class RegisterModeratorRequest
{
    public int EmployeeId { get; set; }

    [Required]
    [MinLength(5), MaxLength(30)]
    public string Username { get; set; }

    [Required]
    [MinLength(5), MaxLength(30)]
    public string Password { get; set; }
}

public class RegisterModeratorResponse
{
    public string Username { get; set; }
    public string Message { get; set; }
    public bool Status { get; set; }
}