using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using Temp.Application.Organizations;
using Temp.Application.Groups;
using Temp.Database;
using Temp.Domain.Models.Groups.Exceptions;
using Temp.Domain.Models.Organizations.Exceptions;
using Microsoft.AspNetCore.Authorization;

namespace Temp.UI.Controllers
{
    [Authorize(Roles = "Admin")]
    public class GroupController : Controller
    {
        public readonly ApplicationDbContext _ctx;

        public GroupController(ApplicationDbContext ctx)
        {
            _ctx = ctx;
        }

        [HttpGet]
        public IActionResult Create(int id)
        {
            try
            {
                TempData["organization"] = new GetOrganization(_ctx).Do(id);
                return View();

            }
            catch (OrganizationValidationException organizationValidationException)
            {
                TempData["message"] = GetInnerMessage(organizationValidationException);
                return View("Edit");
            }
            catch (NullOrganizationException nullOrganizationException)
            {
                TempData["message"] = GetInnerMessage(nullOrganizationException);
                return View("Edit");
            }
            catch (OrganizationServiceException organizationServiceException)
            {
                TempData["message"] = GetInnerMessage(organizationServiceException);
                return View("Edit");
            }
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

        [HttpGet]
        public IActionResult Edit(int id)
        {
            try
            {
                var group = new GetGroup(_ctx).Do(id);
                TempData["organization"] = new GetOrganization(_ctx).Do(group.OrganizationId);
               
                return View(group);
            }
            catch (GroupValidationException groupValidationException)
            {
                TempData["message"] = GetInnerMessage(groupValidationException);
                return View("Edit");
            }
            catch (OrganizationValidationException organizationValidationException)
            {
                TempData["message"] = GetInnerMessage(organizationValidationException);
                return View("Edit");
            }
            catch(NullOrganizationException nullOrganizationException)
            {
                TempData["message"] = GetInnerMessage(nullOrganizationException);
                return View("Edit");
            }
            catch(OrganizationServiceException organizationServiceException)
            {
                TempData["message"] = GetInnerMessage(organizationServiceException);
                return View("Edit");
            }
        }

        [HttpPost]
        public async Task<IActionResult> Edit(UpdateGroup.Request request)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var response = await new UpdateGroup(_ctx).Do(request);
                    if (response.Status)
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
                catch(GroupValidationException groupValidationException)
                {
                    TempData["message"] = GetInnerMessage(groupValidationException);
                }
            }
            return View("Edit", request.Id);
        }

        private static string GetInnerMessage(Exception exception) =>
            exception.InnerException.Message;

    }
}
