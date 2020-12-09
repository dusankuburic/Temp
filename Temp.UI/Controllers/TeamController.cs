using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Temp.Application.Groups;
using Temp.Application.Teams;
using Temp.Database;

namespace Temp.UI.Controllers
{
    public class TeamController : Controller
    {
        private readonly ApplicationDbContext _ctx;

        public TeamController(ApplicationDbContext ctx)
        {
            _ctx = ctx;
        }

        [HttpGet]
        public IActionResult Create(int id)
        {
            TempData["group"] = new GetGroup(_ctx).Do(id);
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateTeam.Request request)
        {
            if(ModelState.IsValid)
            {
                var response = await new CreateTeam(_ctx).Do(request);

                if(response.Status)
                {
                    TempData["success_message"] = response.Message;
                }
                else
                {
                    TempData["message"] = response.Message;
                }

                return RedirectToAction("Create");
            }
            return RedirectToAction("Create");
        }
    }
}
