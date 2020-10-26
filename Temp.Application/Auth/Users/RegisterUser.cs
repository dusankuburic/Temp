using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Temp.Application.Empolyees;
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
                    Messsage = $"User already exists with {request.Username} username",
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
                PasswordSalt = passwordSalt,
                EmployeeId = request.EmpoyeeId
            };

            _ctx.Users.Add(user);
            await _ctx.SaveChangesAsync();

            var result = await new UpdateEmployeeRole(_ctx).Do("User",request.EmpoyeeId);

            return new Response
            {
                Messsage = "Successful registration",
                Username = user.Username,
                Status = true
            };
        }

        public class Request
        {
            public int EmpoyeeId {get; set;}

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
