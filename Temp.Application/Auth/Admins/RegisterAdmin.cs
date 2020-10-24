using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Temp.Application.Empolyees;
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
            using(var hmac = new HMACSHA512())
            {
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
            }
        }

        private async ValueTask<bool> AdminExists(string username)
        {
            if (await _ctx.Admins.AnyAsync(x => x.Username == username))
            {
                return true;
            }
            return false;
        }

        public async ValueTask<Response> Do(Request request)
        {
            var adminExists = await AdminExists(request.Username);

            if (adminExists)
            {
                return new Response
                {
                    Message = $"Admin aready exists with {request.Username} username",
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
                EmployeeId = request.EmpoyeeId
            };

            _ctx.Admins.Add(admin);
            await _ctx.SaveChangesAsync();

            var result = new UpdateEmployeeRole(_ctx).Do("Admin",request.EmpoyeeId);
            

            return new Response
            {
                Message = "Successful registration",
                Username = admin.Username,
                Status = true
            };

        }

        public class Request
        {
            public int EmpoyeeId {get; set;}

            [Required]
            [MinLength(5)]
            public string Username { get; set; }
            [Required]
            [MinLength(5)]
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
