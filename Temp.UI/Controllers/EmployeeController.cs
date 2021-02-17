using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Temp.Application.Employees;
using Temp.Database;
using Temp.Domain.Models.Employees.Exceptions;

namespace Temp.UI.Controllers
{
    [Authorize(Roles = "Admin")]
    public class EmployeeController : Controller
    {
        public readonly ApplicationDbContext _ctx;

        public EmployeeController(ApplicationDbContext ctx)
        {
            _ctx = ctx;
        }

        public IActionResult Index()
        {       
 
            try
            {
                var employees = new GetEmployees(_ctx).Do();
                return View(employees);

            }
            catch(EmployeeValidationException employeeValidationException)
            {
                TempData["message"] = GetInnerMessage(employeeValidationException);
                return View();
            }     
            
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateEmployee.Request request)
        {
            if(ModelState.IsValid)
            {
                try
                {
                    var response = await new CreateEmployee(_ctx).Do(request);

                
                        TempData["success_message"] = response.Message;
                        return RedirectToAction("Create");
                    
  
                }
                catch(EmployeeValidationException employeeValidationException)
                {
                     TempData["message"] = GetInnerMessage(employeeValidationException);
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
                var response = new GetEmployee(_ctx).Do(id);
                return View(response);
            }
            catch(EmployeeValidationException employeeValidationException)
            {
                TempData["message"] = GetInnerMessage(employeeValidationException);
                return View("Index");
            }
        }

        /*
        [HttpPost]
        public async Task<IActionResult> Edit(UpdateEmployee.Request request)
        {       
            if(ModelState.IsValid)
            {
                try
                {
                    var response = await new UpdateEmployee(_ctx).Do(request);
                    if(response.Status)
                    {
                        TempData["success_message"] = response.Message;
                        return RedirectToAction("Edit", response.Id);
                    }
                }
                catch(EmployeeValidationException employeeValidationException)
                {
                    TempData["message"] = GetInnerMessage(employeeValidationException);
                    return RedirectToAction("Edit", request.Id);
                }    
            }
            return View("Edit", request.Id);
        }
        */

        [HttpGet]
        public IActionResult AssignRole(int id)
        {
            TempData["employee"] = new GetEmployee(_ctx).Do(id);
            return View();
        }

        /*
        [HttpPost]
        public async Task<IActionResult> AssignRole(AssignRole.Request request)
        {
            if(ModelState.IsValid)
            {
                var response = await new AssignRole(_ctx).Do(request);

                if(response.Status)
                {
                    TempData["success_message"] = response.Message;
                    return RedirectToAction("Index");
                }
                else
                {
                    TempData["message"] = response.Message;
                    return RedirectToAction("AssignRole", request.Id);
                }
            }
             return View("AssignRole", request.Id);
        }

*/
        [HttpPost]
        public async Task<IActionResult> RemoveRole(RemoveEmployeeRole.Request request)
        {
            if(ModelState.IsValid)
            {
                var response = await new RemoveEmployeeRole(_ctx).Do(request);

                if(response.Status)
                {
                    TempData["success_message"] = response.Message;           
                }
                else
                {
                    TempData["message"] = response.Message;
                }
                 return RedirectToAction("Index");
            }
            return View("Index");
        }
        


        private static string GetInnerMessage(Exception exception) =>
            exception.InnerException.Message;

    }
}
