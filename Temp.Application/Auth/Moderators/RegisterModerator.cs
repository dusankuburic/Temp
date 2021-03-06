﻿using System.ComponentModel.DataAnnotations;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Temp.Application.Employees;
using Temp.Database;
using Temp.Domain.Models;

namespace Temp.Application.Auth.Moderators
{
    public class RegisterModerator
    {
        private readonly ApplicationDbContext _ctx;

        public RegisterModerator(ApplicationDbContext ctx)
        {
            _ctx = ctx;
        }

        private void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            using (var hmac = new HMACSHA512())
            {
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
            }
        }

        private async Task<bool> ModeratorExists(string username)
        {
            if (await _ctx.Moderators.AnyAsync(x => x.Username == username))      
                return true;
            
            return false;
        }

        public async Task<Response> Do(Request request)
        {
            var moderatorExists = await ModeratorExists(request.Username);

            if (moderatorExists)
            {
                return new Response
                {
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

            var result = await new UpdateEmployeeRole(_ctx).Do("Moderator", request.EmployeeId);

            return new Response
            {
                Message = "Successful registration",
                Username = moderator.Username,
                Status = true
            };
        }
        

        public class Request
        {
            public int EmployeeId { get; set; }

            [Required]
            [MinLength(5),MaxLength(30)]
            public string Username { get; set; }
            
            [Required]
            [MinLength(5),MaxLength(30)]
            public string Password { get; set; }
        }

        public class Response
        {
            public  string Username { get; set; }
            public string Message { get; set; }
            public bool Status { get; set; }
        }
    }
}