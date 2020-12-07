using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Temp.Application.Organizations;
using Temp.Application.Groups;
using Temp.Database;

namespace Temp.UI.Controllers
{
    public class GroupController : Controller
    {
        public readonly ApplicationDbContext _ctx;

        public GroupController(ApplicationDbContext ctx)
        {
            _ctx = ctx;
        }

        public IActionResult Index()
        {
            return View();
        }


        [HttpGet]
        public IActionResult Create(int id)
        {
            TempData["organization"] = new GetOrganization(_ctx).Do(id);
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateGroup.Request request)
        {
            if (ModelState.IsValid)
            {
                var response = await new CreateGroup(_ctx).Do(request);

                if (response.Status)
                {
                    TempData["success_message"] = response.Message;
                    return RedirectToAction("Create");
                }
                else
                {
                    TempData["message"] = response.Message;
                    return RedirectToAction("Create");
                }
            }
            return RedirectToAction("Create");
        }

    }
}
