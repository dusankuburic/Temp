using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Temp.Application.Auth.Admins;
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
    }
}
