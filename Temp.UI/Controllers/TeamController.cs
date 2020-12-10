using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using Temp.Application.Groups;
using Temp.Application.Teams;
using Temp.Database;
using Temp.Domain.Models.Groups.Exceptions;
using Temp.Domain.Models.Teams.Exceptions;

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
            try
            {
                TempData["group"] = new GetGroup(_ctx).Do(id);
                return View();
            }
            catch(GroupValidationException groupValidationException)
            {
                TempData["message"] = GetInnerMessage(groupValidationException);
                return View("Create");
            }
            catch(NullGroupException nullGroupException)
            {
                TempData["message"] = GetInnerMessage(nullGroupException);
                return View("Create");
            }
            catch(GroupServiceException groupServiceException)
            {
                TempData["message"] = GetInnerMessage(groupServiceException);
                return View("Create");
            }
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateTeam.Request request)
        {
            if(ModelState.IsValid)
            {
                try
                {
                    var response = await new CreateTeam(_ctx).Do(request);

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
                catch(TeamValidationException teamValidationException)
                {
                    TempData["message"] = GetInnerMessage(teamValidationException);
                }
            }
            return RedirectToAction("Create");
        }

        private static string GetInnerMessage(Exception exception) =>
            exception.InnerException.Message;
    }
}
