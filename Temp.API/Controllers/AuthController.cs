using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Temp.Application.Auth.Admins;
using Temp.Database;

namespace Temp.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly ApplicationDbContext _ctx;
        private readonly IConfiguration _config;

        public AuthController(ApplicationDbContext ctx, IConfiguration config)
        {
            _ctx = ctx;
            _config = config;
        }


        [HttpPost("register")]
        public async Task<IActionResult> RegisterAdmin(RegisterAdmin.Request request)
        {
            var response = await new RegisterAdmin(_ctx).Do(request);
            if (response.Status)
            {
                return Ok(response);
            }

            return BadRequest(response);
        }
        
      
        [HttpPost("login")]
        public async Task<IActionResult> LoginAdmin(LoginAdmin.Request request)
        {

            if(!ModelState.IsValid)
            {
                return BadRequest(ModelState.Values);
            }
            
            var response = await new LoginAdmin(_ctx, _config).Do(request);
            if (response is null)
                return Unauthorized();
            
            return Ok(response);
        }

        
/*


        [HttpPost]
        public async Task<IActionResult> RegisterUser(RegisterUser.Request request)
        {
            if(ModelState.IsValid)
            {
                var response = await new RegisterUser(_ctx).Do(request);

                if(response.Status)
                {
                    SetUpUserClaims(response.Username);
                    return RedirectToAction("Index", "User");
                }
                else
                {
                    TempData["message"] = response.Messsage;
                }
            }

            return View("RegisterUser");
        }


        [HttpPost]
        public async Task<IActionResult> LoginUser(LoginUser.Request request)
        {
            if(ModelState.IsValid)
            {
                var response = await new LoginUser(_ctx).Do(request);

                if (response.Status)
                {
                    SetUpUserClaims(response.Username);
                    return RedirectToAction("Index", "User");
                }
                else
                {
                    TempData["message"] = response.Message;
                }
            }

            return View("LoginUser");
        }
        */


        /*

        public IActionResult Logout()
        {
            foreach(var cookie in HttpContext.Request.Cookies)
            {
                Response.Cookies.Delete(cookie.Key);
            }

            return RedirectToAction("Index","Auth");
        }
*/


        private void SetUpUserClaims(string username)
        {
            var userClaims = new List<Claim>()
            {
                new Claim(ClaimTypes.Name, username),
                new Claim(ClaimTypes.Role, "User")
            };

            var userIdentity = new ClaimsIdentity(userClaims, "User_Identity");
            var userPrincipal = new ClaimsPrincipal(userIdentity);

            HttpContext.SignInAsync(userPrincipal);
        }
    }
}