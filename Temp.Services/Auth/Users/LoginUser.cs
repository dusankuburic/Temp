﻿using Temp.Database;

namespace Temp.Services.Auth.Users;

public class LoginUser
{
    private readonly ApplicationDbContext _ctx;
    private readonly IConfiguration _config;

    public LoginUser(ApplicationDbContext ctx, IConfiguration config) {
        _ctx = ctx;
        _config = config;
    }

    public bool VerifyPasswordHash(string password, byte[] passwordHash, byte[] passwordSalt) {
        using (var hmac = new HMACSHA512(passwordSalt)) {
            var computerHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
            for (int i = 0; i < computerHash.Length; i++) {
                if (computerHash[i] != passwordHash[i])
                    return false;
            }
        }
        return true;
    }

    public async Task<Response> Do(LoginUserRequest request) {
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

}


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
