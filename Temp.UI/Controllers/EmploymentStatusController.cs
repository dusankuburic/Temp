using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Temp.Application.EmploymentStatuses;
using Temp.Database;
using Temp.Domain.Models.EmploymentStatuses.Exceptions;

namespace Temp.UI.Controllers
{
    public class EmploymentStatusController : Controller
    {

        private readonly ApplicationDbContext _ctx;

        public EmploymentStatusController(ApplicationDbContext ctx)
        {
            _ctx = ctx;
        }

        public IActionResult Index()
        {
            try
            {
                var employmentStatuses = new GetEmploymentStatuses(_ctx).Do();
                return View(employmentStatuses);
            }
            catch(EmploymentStatusValidationException employmentStatusValidationException)
            {
                TempData["message"] = GetInnerMesage(employmentStatusValidationException);
                return View();
            }
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateEmploymentStatus.Request request)
        {
            if (ModelState.IsValid)
            {
                try
                {               
                    var response = await new CreateEmploymentStatus(_ctx).Do(request); 
                
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
                catch(EmploymentStatusValidationException employmentStatusValidationException)
                {
                    TempData["message"] = GetInnerMesage(employmentStatusValidationException);
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
                var response = new GetEmploymentStatus(_ctx).Do(id);
                return View(response);
            }
            catch(EmploymentStatusValidationException employmentStatusValidationException)
            {
                TempData["message"] = GetInnerMesage(employmentStatusValidationException);
                return View("Index");
            }            
        }


        [HttpPost]
        public async Task<IActionResult> Edit(UpdateEmploymentStatus.Request request)
        {
            if(ModelState.IsValid)
            {
                try
                {
                    var response = await new UpdateEmploymentStatus(_ctx).Do(request);

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
                catch(EmploymentStatusValidationException employmentStatusValidationException)
                {
                    TempData["message"] = GetInnerMesage(employmentStatusValidationException);
                    return RedirectToAction("Edit", request.Id);
                }
            }
            return View("Edit", request.Id);

        }

        private static string GetInnerMesage(Exception exception) =>
            exception.InnerException.Message;
    }
}
