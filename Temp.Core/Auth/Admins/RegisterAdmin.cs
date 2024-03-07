﻿using Temp.Database;
using Temp.Domain.Models;

namespace Temp.Core.Auth.Admins;

public class RegisterAdmin
{
    private readonly ApplicationDbContext _ctx;

    public RegisterAdmin(ApplicationDbContext ctx) {
        _ctx = ctx;
    }

    private void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt) {
        using (var hmac = new HMACSHA512()) {
            passwordSalt = hmac.Key;
            passwordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
        }
    }

    private async Task<bool> AdminExists(string username) {
        return await _ctx.Admins.AnyAsync(x => x.Username == username);
    }

    public async Task<RegisterAdminResponse> Do(RegisterAdminRequest request) {
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

        //var result = await new UpdateEmployeeRole(_ctx).Do("Admin",request.EmployeeId);

        return new RegisterAdminResponse {
            Message = "Successful registration",
            Username = admin.Username,
            Status = true
        };
    }



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
