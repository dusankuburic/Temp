using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using Temp.Database;

namespace Temp.UI.Controllers
{
    [Authorize(Roles = "Admin")]
    public class OrganizationController : Controller
    {
        private readonly ApplicationDbContext _ctx;

        public OrganizationController(ApplicationDbContext ctx)
        {
            _ctx = ctx;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }


        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }


    }
}
