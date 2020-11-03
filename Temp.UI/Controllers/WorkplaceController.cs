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
            try
            {
                var workplaces = new GetWorkplaces(_ctx).Do();
                return View(workplaces);

            }
            catch(WorkplaceValidationException workplaceValidationException)
            {
                TempData["message"] = GetInnerMessage(workplaceValidationException);
                return View();
            }
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
            try
            {
                var response = new GetWorkplace(_ctx).Do(id);
                return View(response);

            }
            catch(WorkplaceValidationException workplaceValidationException)
            {
                TempData["message"] = GetInnerMessage(workplaceValidationException);
                return View("Index");
            }
        }

        [HttpPost]
        public async Task<IActionResult> Edit(UpdateWorkplace.Request request)
        {
            if(ModelState.IsValid)
            {
                try
                {                
                    var response = await new UpdateWorkplace(_ctx).Do(request);
                    if(response.Status)
                    {
                        TempData["success_message"] = response.Message;
                        return RedirectToAction("Edit", response.Id);
                    }
                }
                catch (WorkplaceValidationException workplaceValidationException)
                { 
                    TempData["message"] = GetInnerMessage(workplaceValidationException);
                    return RedirectToAction("Edit", request.Id);
                }
            }
            return View("Edit", request.Id);
        }

       
        private static string GetInnerMessage(Exception exception) =>
            exception.InnerException.Message;
    }
}
