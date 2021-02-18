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
    public class EmployeesController : ControllerBase
    {
        private readonly ApplicationDbContext _ctx;

        public EmployeesController(ApplicationDbContext ctx)
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
        
        [HttpGet("{id}")]
        public GetEmployee.EmployeeViewModel GetEmployee(int id)
        {
            return new GetEmployee(_ctx).Do(id);
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

        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateEmployee(int id, UpdateEmployee.Request request)
        {
            var response = await new UpdateEmployee(_ctx).Do(id,request);
            if (response.Status)
                return NoContent();

            return BadRequest();
        }
        
        [HttpPost("assign")]
        public async Task<IActionResult> AssignRole(AssignRole.Request request)
        {
            var response = await new AssignRole(_ctx).Do(request);
            if (response.Status)
                return Ok();
            
            return BadRequest(response.Message);
        }
        
        [HttpPost("unassign")]
        public async Task<IActionResult> RemoveRole(RemoveEmployeeRole.Request request)
        {
            var response = await new RemoveEmployeeRole(_ctx).Do(request);
            if (response.Status)
                return Ok();
            
            return BadRequest(response.Message);
        }
        
        private static string GetInnerMessage(Exception exception) =>
            exception.InnerException.Message;

        }
    
}
