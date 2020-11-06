using System;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Temp.Application.Empolyees;
using Temp.Database;
using Temp.Domain.Models.Employees.Exceptions;

namespace Temp.UI.Controllers
{
    [Authorize(Roles = "Admin")]
    public class EngagementController : Controller
    {
        public readonly ApplicationDbContext _ctx;

        public EngagementController(ApplicationDbContext ctx)
        {
            _ctx = ctx;
        }

        public IActionResult Index()
        {
            try
            {
                var employees = new GetEmployeesWithoutEngagement(_ctx).Do();
                return View(employees);
            }
            catch (EmployeeValidationException employeeValidationException)
            {
                TempData["message"] = GetInnerMessage(employeeValidationException);
                return View();
            }
        }


        private static string GetInnerMessage(Exception exception) =>
        exception.InnerException.Message;
    }
}
