using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Temp.Application.Auth.Admins;
using Temp.Application.Auth.Users;
using Temp.Database;

namespace Temp.UI.Controllers
{
    public class AuthController : Controller
    {
        public readonly ApplicationDbContext _ctx;

        public AuthController(ApplicationDbContext ctx)
        {
            _ctx = ctx;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult RegisterAdmin()
        {
            return View();
        }

        //TODO: Pass Status value to View so program can decide what color to use based on success 

        [HttpPost]
        public async Task<IActionResult> RegisterAdmin(RegisterAdmin.Request request)
        {
            if (ModelState.IsValid)
            {
                var response = await new RegisterAdmin(_ctx).Do(request);
                TempData["message"] = response.Message;

                if (response.Status)
                {
                    SetUpAdminClaims(response.Username);
                    return RedirectToAction("Index", "Admin");
                }
            }

            return View("RegisterAdmin");
        }

        [HttpGet]
        public IActionResult LoginAdmin()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> LoginAdmin(LoginAdmin.Request request)
        {
            if(ModelState.IsValid)
            {
                var response = await new LoginAdmin(_ctx).Do(request);
                TempData["message"] = response.Mesasge;

                if(response.Status)
                {
                    SetUpAdminClaims(response.Username);
                    return RedirectToAction("Index", "Admin");
                }
            }

            return View("LoginAdmin");
        }


        [HttpGet]
        public IActionResult RegisterUser()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> RegisterUser(RegisterUser.Request request)
        {
            if(ModelState.IsValid)
            {
                var response = await new RegisterUser(_ctx).Do(request);
                TempData["message"] = response.Messsage;

                if(response.Status)
                {
                    SetUpUserClaims(response.Username);
                    return RedirectToAction("Index", "User");
                }
            }

            return View("RegisterUser");
        }

        [HttpGet]
        public IActionResult LoginUser()
        {
            return View("LoginUser");
        }

        [HttpPost]
        public async Task<IActionResult> LoginUser(LoginUser.Request request)
        {
            if(ModelState.IsValid)
            {
                var response = await new LoginUser(_ctx).Do(request);
                TempData["message"] = response.Message;

                if(response.Status)
                {
                    SetUpUserClaims(response.Username);
                    return RedirectToAction("Index", "User");
                }
            }

            return View("LoginUser");
        }


        [HttpPost]
        public IActionResult Logout()
        {
            foreach(var cookie in HttpContext.Request.Cookies)
            {
                Response.Cookies.Delete(cookie.Key);
            }

            return RedirectToAction("Index","Auth");
        }

        private void SetUpAdminClaims(string username)
        {
            var adminClaims = new List<Claim>()
                {
                    new Claim(ClaimTypes.Name,  username),
                    new Claim(ClaimTypes.Role, "Admin")
                };

            var adminIdentity = new ClaimsIdentity(adminClaims, "Admin Identity");
            var adminPrincipal = new ClaimsPrincipal(adminIdentity);

            HttpContext.SignInAsync(adminPrincipal);
        }

        private void SetUpUserClaims(string username)
        {
            var userClaims = new List<Claim>()
                {
                    new Claim(ClaimTypes.Name,  username),
                    new Claim(ClaimTypes.Role, "User")
                };

            var userIdentity = new ClaimsIdentity(userClaims, "User Identity");
            var userPrincipal = new ClaimsPrincipal(userIdentity);

            HttpContext.SignInAsync(userPrincipal);
        }
    }
}
