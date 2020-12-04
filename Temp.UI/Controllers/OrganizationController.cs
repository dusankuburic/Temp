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
            return View();
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

        private static string GetInnerMessage(Exception exception) =>
            exception.InnerException.Message;
    }
}
