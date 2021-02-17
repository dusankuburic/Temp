using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Temp.Application.Employees;
using Temp.Database;
using Temp.Domain.Models.Employees.Exceptions;

namespace Temp.API.Controllers
{
    [Authorize(Roles = "Admin")]
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeeController : ControllerBase
    {
        private readonly ApplicationDbContext _ctx;

        public EmployeeController(ApplicationDbContext ctx)
        {
            _ctx = ctx;
        }

        [HttpGet]
        public ActionResult<IEnumerable<GetEmployees.EmployeeViewModel>> GetEmployees()
        {
            try
            {
                var response = new GetEmployees(_ctx).Do();
                return Ok(response);

            }
            catch (EmployeeValidationException employeeValidationException)
            {
                return BadRequest(new
                {
                    Message = GetInnerMessage(employeeValidationException)
                });
            }
        }
        
        [HttpPost]
        public async Task<ActionResult> Create(CreateEmployee.Request request)
        {
            try
            {
                var response = await new CreateEmployee(_ctx).Do(request);
                return Ok(response);
            }
            catch (EmployeeValidationException employeeValidationException)
            {
                return BadRequest(new CreateEmployee.Response
                {
                    Message = GetInnerMessage(employeeValidationException)
                });
            }      
        }

        [HttpGet("{id}")]
        public GetEmployee.EmployeeViewModel Edit(int id)
        {
           return new GetEmployee(_ctx).Do(id);
        }
           
   
        [HttpPut]
        public async Task<UpdateEmployee.Response> Edit(UpdateEmployee.Request request)
        {
            var response = await new UpdateEmployee(_ctx).Do(request);
            return response;
        }

    /*
    [HttpPost]
    public async Task<IActionResult> AssignRole(AssignRole.Request request)
    {
        if (ModelState.IsValid)
        {
            var response = await new AssignRole(_ctx).Do(request);

            if (response.Status)
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


    [HttpPost]
    public async Task<IActionResult> RemoveRole(RemoveEmployeeRole.Request request)
    {
        if (ModelState.IsValid)
        {
            var response = await new RemoveEmployeeRole(_ctx).Do(request);

            if (response.Status)
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
    */
    
    private static string GetInnerMessage(Exception exception) =>
        exception.InnerException.Message;

    }
    
}
