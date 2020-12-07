using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Temp.Application.Organizations;
using Temp.Application.Groups;
using Temp.Database;
using Temp.Domain.Models.Groups.Exceptions;

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
                try
                {
                    var response = await new CreateGroup(_ctx).Do(request);

                    if (response.Status)
                    {
                        TempData["success_message"] = response.Message;
                    }
                    else
                    {
                        TempData["message"] = response.Message;                  
                    }

                    return RedirectToAction("Create");

                }
                catch(GroupValidationException groupValidationException)
                {
                    TempData["message"] = GetInnerMessage(groupValidationException);
                    return RedirectToAction("Create");
                }
            }
            return RedirectToAction("Create");
        }

        private static string GetInnerMessage(Exception exception) =>
            exception.InnerException.Message;

    }
}
