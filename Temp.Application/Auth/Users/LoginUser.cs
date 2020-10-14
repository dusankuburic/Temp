using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Temp.Database;

namespace Temp.Application.Auth.Users
{
    public class LoginUser
    {
        public readonly ApplicationDbContext _ctx;

        public LoginUser(ApplicationDbContext ctx)
        {
            _ctx = ctx;
        }

        public bool VerifyPasswordHash(string password, byte[] passwordHash, byte[] passwordSalt)
        {
            using(var hmac = new HMACSHA512(passwordSalt))
            {
                var computerHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
                for(int i = 0; i < computerHash.Length; i++)
                {
                    if (computerHash[i] != passwordHash[i])
                        return false;
                }
            }
            return true;
        }

        public async ValueTask<Response> Do(Request request)
        {
            var user = await _ctx.Users.FirstOrDefaultAsync(x => x.Username == request.Username);

            if(user == null)
            {
                return new Response
                {
                    Username = request.Username,
                    Message = "Error",
                    Status = false
                };
            }

            if(!VerifyPasswordHash(request.Password, user.PasswordHash, user.PasswordSalt))
            {
                return new Response
                {
                    Username = request.Username,
                    Message = "Password mismatch",
                    Status = false
                };
            }

            return new Response
            {
                Username = user.Username,
                Message = "Success",
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
            public string Message { get; set; }
            public bool Status { get; set; }
        }
    }
}
