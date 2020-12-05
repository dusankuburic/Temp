using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using Temp.Application.Organizations;
using Temp.Domain.Models.Organizations.Exceptions;
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
            try
            {
                var organizations = new GetOrganizations(_ctx).Do();
                return View(organizations);
            }
            catch(OrganizationValidationException organizationValidationException)
            {
                TempData["message"] = GetInnerMessage(organizationValidationException);
                return View();
            }
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateOrganization.Request request)
        {
            if(ModelState.IsValid)
            {
                try
                {
                    var response = await new CreateOrganization(_ctx).Do(request);

                    if(response.Status)
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
                catch(OrganizationValidationException organizationValidationException)
                {
                    TempData["message"] = GetInnerMessage(organizationValidationException);
                    return RedirectToAction("Create");
                }
            }
            return View("Create");
        }

        [HttpGet]
        public IActionResult Edit(int id)
        {
            try
            {
                var response = new GetOrganization(_ctx).Do(id);
                return View(response);
            }
            catch(OrganizationValidationException organizationValidationException)
            {
                TempData["message"] = GetInnerMessage(organizationValidationException);
                return View("Index");
            }
        }

        [HttpPost]
        public async Task<IActionResult> Edit(UpdateOrganization.Request request)
        {
            if(ModelState.IsValid)
            {
                try
                {
                    var response = await new UpdateOrganization(_ctx).Do(request);
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
                catch(OrganizationValidationException organizationValidationException)
                {
                    TempData["message"] = GetInnerMessage(organizationValidationException);
                    return RedirectToAction("Edit", request.Id);
                }
            }

            return View("Edit", request.Id);
        }

        private static string GetInnerMessage(Exception exception) =>
            exception.InnerException.Message;
    }
}
