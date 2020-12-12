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

        [HttpGet]
        public IActionResult Edit(int id)
        {
            try
            {
                var team = new GetTeam(_ctx).Do(id);
                TempData["group"] = new GetGroup(_ctx).Do(team.GroupId);

                return View(team);
            }
            catch(TeamValidationException teamValidationException)
            {
                TempData["message"] = GetInnerMessage(teamValidationException);
                return View("Edit");
            }
            catch(GroupValidationException groupValidationException)
            {
                TempData["message"] = GetInnerMessage(groupValidationException);
                return View("Edit");
            }
            catch (NullGroupException nullGroupException)
            {
                TempData["message"] = GetInnerMessage(nullGroupException);
                return View("Edit");
            }
            catch (GroupServiceException groupServiceException)
            {
                TempData["message"] = GetInnerMessage(groupServiceException);
                return View("Edit");
            }
        }


        [HttpPost]
        public async Task<IActionResult> Edit(UpdateTeam.Request request)
        {
            if(ModelState.IsValid)
            {   
                try
                {
                    var response = await new UpdateTeam(_ctx).Do(request);
                    if(response.Status)
                    {
                        TempData["success_message"] = response.Message;
                        return RedirectToAction("Edit", response.Id);
                    }
                    else
                    {
                        TempData["message"] = response.Message;
                        return RedirectToAction("Edit", response.Id);
                    }

                }
                catch(TeamValidationException treamValidationException)
                {
                    TempData["message"] = GetInnerMessage(treamValidationException);
                }
            }

            return View("Edit", request.Id);
        }

        private static string GetInnerMessage(Exception exception) =>
            exception.InnerException.Message;
    }
}
