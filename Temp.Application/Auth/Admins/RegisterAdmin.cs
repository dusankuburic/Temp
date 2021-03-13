using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Temp.Application.Employees;
using Temp.Database;
using Temp.Domain.Models;

namespace Temp.Application.Auth.Admins
{
    public class RegisterAdmin
    {
        private readonly ApplicationDbContext _ctx;

        public RegisterAdmin(ApplicationDbContext ctx)
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

        private async Task<bool> AdminExists(string username)
        {
            if (await _ctx.Admins.AnyAsync(x => x.Username == username))
            {
                return true;
            }

            return false;
        }

        public async Task<Response> Do(Request request)
        {
            var adminExists = await AdminExists(request.Username);

            if (adminExists)
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

            var admin = new Admin
            {
                Username = request.Username,
                PasswordHash = passwordHash,
                PasswordSalt = passwordSalt,
                EmployeeId = request.EmployeeId
            };

            _ctx.Admins.Add(admin);
            await _ctx.SaveChangesAsync();

             var result = await new UpdateEmployeeRole(_ctx).Do("Admin",request.EmployeeId);

            return new Response
            {
                Message = "Successful registration",
                Username = admin.Username,
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
            public string Username { get; set; }
            
            public string Message { get; set; }
            
            public bool Status { get; set; }
        }
    }
}