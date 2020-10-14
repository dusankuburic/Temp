using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Temp.Database;

namespace Temp.Application.Auth.Admins
{
    public class LoginAdmin
    {
        private readonly ApplicationDbContext _ctx;

        public LoginAdmin(ApplicationDbContext ctx)
        {
            _ctx = ctx;
        }

        private bool VerifyPasswordHash(string password, byte[] passwordHash, byte[] passwordSalt)
        {
            using(var hmac = new HMACSHA512(passwordSalt))
            {
                var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
                for(int i = 0; i < computedHash.Length; i++)
                {
                    if (computedHash[i] != passwordHash[i])
                        return false;
                }
            }
            return true;
        }

        public async ValueTask<Response> Do(Request request)
        {
            var admin = await _ctx.Admins.FirstOrDefaultAsync(x => x.Username == request.Username);

            if(admin == null)
            {
                return new Response
                {
                    Username = request.Username,
                    Mesasge = "Error",
                    Status = false
                };
            }

            if(!VerifyPasswordHash(request.Password, admin.PasswordHash, admin.PasswordSalt))
            {
                return new Response
                {
                    Username = request.Username,
                    Mesasge = "Password mismatch",
                    Status = false
                };
            }

            return new Response
            {
                Username = admin.Username,
                Mesasge = "Success",
                Status = true
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

        public class Response
        {
            public string Username { get; set; }
            public string Mesasge { get; set; }
            public bool Status { get; set; }
        }
    }
}
