﻿using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Temp.Database;

namespace Temp.Application.Auth.Admins
{
    public class LoginAdmin
    {
        private readonly ApplicationDbContext _ctx;
        private readonly IConfiguration _config;

        public LoginAdmin(ApplicationDbContext ctx, IConfiguration config)
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
            var admin = await _ctx.Admins.FirstOrDefaultAsync(x => x.Username == request.Username);

            if (admin is null)
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
                Expires = DateTime.Now.AddDays(1),
                SigningCredentials = creds
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);

            return new Response
            {
                User = new AdminResponse
                {
                    Id = admin.Id,
                    Username = admin.Username          
                },
                Token = tokenHandler.WriteToken(token)
            };
        }
        

        public class Request
        {
            [Required]
            [MaxLength(30)]
            public string Username { get; set; }
            [Required]
            [MaxLength(30)]
            public string Password { get; set; }
        }
        
        public class AdminResponse
        {
            public int Id { get; set; }
            public string Username { get; set; }
        }

        public class Response
        {
            public string Token { get; set; }
            public AdminResponse User { get; set; }
        }
        
    }
}
