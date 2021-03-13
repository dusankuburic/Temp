using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Temp.Database;

namespace Temp.Application.Auth.Moderators
{
    public class LoginModerator
    {
        private readonly ApplicationDbContext _ctx;
        private readonly IConfiguration _config;

        public LoginModerator(ApplicationDbContext ctx, IConfiguration config)
        {
            _ctx = ctx;
            _config = config;
        }

        private bool VerifyPasswordHash(string password, byte[] passwordHash, byte[] passwordSalt)
        {
            using (var hmac = new HMACSHA512(passwordSalt))
            {
                var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
                for (int i = 0; i < computedHash.Length; i++)
                {
                    if (computedHash[i] != passwordHash[i])
                        return false;
                }
            }
            return true;
        }

        public async Task<Response> Do(Request request)
        {
            var moderator = await _ctx.Moderators.FirstOrDefaultAsync(x => x.Username == request.Username);

            if (moderator is null)
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
                Expires = DateTime.Now.AddDays(1),
                SigningCredentials = creds
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);

            return new Response
            {   
                User = new ModeratorResponse
                {
                    Id = moderator.Id,
                    Username = moderator.Username
                },
                Token = tokenHandler.WriteToken(token)
            };
        }
        

        public class Request
        {
            [Required]
            [MaxLength(30)]
            public  string Username { get; set; }
            
            [Required]
            [MaxLength(30)]
            public  string Password { get; set; }
        }

        public class ModeratorResponse
        {
            public  int Id { get; set; }
            public string Username { get; set; }
        }

        public class Response
        {
            public  string Token { get; set; }
            public ModeratorResponse User { get; set; }
        }
    }
}