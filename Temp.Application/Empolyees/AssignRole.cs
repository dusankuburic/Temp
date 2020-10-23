using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Temp.Application.Auth.Admins;
using Temp.Application.Auth.Users;
using Temp.Database;


namespace Temp.Application.Empolyees
{
    public class AssignRole
    {
        private readonly ApplicationDbContext _ctx;

        public AssignRole(ApplicationDbContext ctx)
        {
            _ctx = ctx;
        }

        public async Task<Response> Do(Request request)
        {




            if (request.Role == "User")
            {   


                var userRequest = new RegisterUser.Request
                {
                    Username = request.Username,
                    Password = request.Password,
                    EmpoyeeId = request.Id
                };

                var response = await new RegisterUser(_ctx).Do(userRequest);

                return new Response
                {
                    Username = response.Username,
                    Message = response.Messsage,
                    Status = response.Status
                };
                
            }
            else if(request.Role == "Admin")
            {
                var adminRequest = new RegisterAdmin.Request
                {
                    Username = request.Username,
                    Password = request.Password,
                    EmpoyeeId = request.Id
                };

                var response = await new RegisterAdmin(_ctx).Do(adminRequest);

                return new Response
                {
                    Username = response.Username,
                    Message = response.Message,
                    Status = response.Status
                };
            }
            else
            {
                return new Response
                {
                    Status = false,
                    Message = "Wrong role!!!!"
                };
            }
            
        }

        public class Request
        {
            [Required]
            public int Id {get; set;}
            [Required]
            [MaxLength(30)]
            public string Username { get; set; }
            [Required]
            [MaxLength(30)]
            public string Password { get; set; }
            [Required]
            [MaxLength(30)]
            public string ConfirmPassword { get; set; }
            [Required]
            public string Role {get; set;}
        }

        public class Response
        {
            public string Username { get; set; }
            public string Message { get; set; }
            public bool Status { get; set; }
        }
    }
}
