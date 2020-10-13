using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Temp.Database;
using Temp.Domain.Models;

namespace Temp.Application.Auth.Users
{
    public class RegisterUser
    {

        private readonly ApplicationDbContext _ctx;

        public RegisterUser(ApplicationDbContext ctx)
        {
            _ctx = ctx;
        }

        private void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            using(var hmac = new HMACSHA512())
            {
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
            }
        }

        private async Task<bool> UserExists(string username)
        {
            if(await _ctx.Users.AnyAsync(x => x.Username == username))
            {
                return true;
            }

            return false;
        }

        public async Task<Response> Do(Request request)
        {
            var userExists = await UserExists(request.Username);

            if(userExists)
            {
                return new Response
                {
                    Messsage = "User already exists with same username",
                    Username = request.Username,
                    Status = false
                };
            }

            byte[] passwordHash, passwordSalt;
            CreatePasswordHash(request.Password, out passwordHash, out passwordSalt);

            var user = new User
            {
                Username = request.Username,
                PasswordHash = passwordHash,
                PasswordSalt = passwordSalt
            };

            _ctx.Users.Add(user);
            await _ctx.SaveChangesAsync();

            return new Response
            {
                Messsage = "Successful registration",
                Username = user.Username,
                Status = true
            };

        }

        public class Request
        {
            [Required]
            public string Username { get; set; }
            [Required]
            public string Password { get; set; }
        }

        public class Response
        {
            public string Username { get; set; }
            public string Messsage { get; set; }
            public bool Status { get; set; }
        }
    }
}
