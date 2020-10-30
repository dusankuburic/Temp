using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Temp.Application.Workplaces;
using Temp.Database;
using Temp.Domain.Models.Workplaces.Exceptions;

namespace Temp.UI.Controllers
{
    [Authorize(Roles = "Admin")]
    public class WorkplaceController : Controller
    {
        public readonly ApplicationDbContext _ctx;

        public WorkplaceController(ApplicationDbContext ctx)
        {
            _ctx = ctx;
        }

        public IActionResult Index()
        {
            return View(new GetWorkplaces(_ctx).Do());
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }


        [HttpPost]
        public async Task<IActionResult> Create(CreateWorkplace.Request request)
        {
            if (ModelState.IsValid)
            {         
                try
                {
                    var response = await new CreateWorkplace(_ctx).Do(request);

                    if(response.Status)
                    {
                        TempData["success_message"] = response.Message;
                        return RedirectToAction("Create");
                    }
                }
                catch(WorkplaceValidationException workplceValidationException)
                {
                    TempData["message"] = GetInnerMessage(workplceValidationException);
                    return RedirectToAction("Create");
                }         
            }
            return View("Create");
        }

        [HttpGet]
        public IActionResult Edit(int id)
        {
            var response = new GetWorkplace(_ctx).Do(id);

            if(response is null)
            {
                return RedirectToAction("Index","Error");
            }

            return View(response);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(UpdateWorkplace.Request request)
        {

            if(ModelState.IsValid)
            {
                var response = await new UpdateWorkplace(_ctx).Do(request);

                if(response.Status)
                {
                    TempData["success_message"] = response.Message;
                    return RedirectToAction("Edit", response.Id);
                }
            }
            return View("Edit", request.Id);
        }

       
        private static string GetInnerMessage(Exception exception) =>
            exception.InnerException.Message;
    }
}
