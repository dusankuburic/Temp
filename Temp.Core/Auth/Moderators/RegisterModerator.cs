using Temp.Database;
using Temp.Domain.Models;

namespace Temp.Core.Auth.Moderators;

public class RegisterModerator
{
    private readonly ApplicationDbContext _ctx;

    public RegisterModerator(ApplicationDbContext ctx) {
        _ctx = ctx;
    }

    private void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt) {
        using (var hmac = new HMACSHA512()) {
            passwordSalt = hmac.Key;
            passwordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
        }
    }

    private async Task<bool> ModeratorExists(string username) {
        return await _ctx.Moderators.AnyAsync(x => x.Username == username);
    }

    public async Task<RegisterModeratorResponse> Do(RegisterModeratorRequest request) {
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

        //var result = await new UpdateEmployeeRole(_ctx).Do("Moderator", request.EmployeeId);

        return new RegisterModeratorResponse {
            Message = "Successful registration",
            Username = moderator.Username,
            Status = true
        };
    }



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