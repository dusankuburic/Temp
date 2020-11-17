﻿using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Temp.Application.Empolyees;
using Temp.Application.Engagements;
using Temp.Database;
using Temp.Domain.Models.Employees.Exceptions;
using Temp.Domain.Models.EmploymentStatuses.Exceptions;
using Temp.Domain.Models.Workplaces.Exceptions;

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

        [HttpGet]
        public IActionResult Index()
        {
            return View("Index");
        }


        [HttpGet]
        public IActionResult WithoutEngagements()
        {
            try
            {
                var employeesWithoutEngagement = new GetEmployeesWithoutEngagement(_ctx).Do();
                return View(employeesWithoutEngagement);
            }
            catch(EmployeeValidationException employeeValidationException)
            {
                TempData["message"] += GetInnerMessage(employeeValidationException);
                return View("WithoutEngagements");
            }
            catch(WorkplaceValidationException workplaceValidationException)
            {
                TempData["message"] += GetInnerMessage(workplaceValidationException);
                return View("WithoutEngagements");
            }
            catch(EmploymentStatusValidationException employmentStatusValidationException)
            {
                TempData["message"] += GetInnerMessage(employmentStatusValidationException);
                return View("WithoutEngagements");
            }
        }


        [HttpGet]
        public IActionResult WithEngagements()
        {
            try
            {
                var employeesWithEngagement = new GetEmployeesWithEngagement(_ctx).Do();
                return View(employeesWithEngagement);
            }
            catch(EmployeeValidationException employeeValidationException)
            {
                TempData["message"] += GetInnerMessage(employeeValidationException);
                return View("WithEngagements");
            }
            catch(WorkplaceValidationException workplaceValidationException)
            {
                TempData["message"] += GetInnerMessage(workplaceValidationException);
                return View("WithEngagements");
            }
            catch(EmploymentStatusValidationException employmentStatusValidationException)
            {
                TempData["message"] += GetInnerMessage(employmentStatusValidationException);
                return View("WithEngagements");
            }
        }

        [HttpGet]
        public IActionResult Create(int id)
        {
            try
            {
                var employeesWithoutEngagement = new GetCreateEngagementViewModel(_ctx).Do(id);
                return View(employeesWithoutEngagement);
            }
            catch(EmployeeValidationException employeeValidationException)
            {
                TempData["message"] += GetInnerMessage(employeeValidationException);
                return View("WithoutEngagements");
            }
            catch(WorkplaceValidationException workplaceValidationException)
            {
                TempData["message"] += GetInnerMessage(workplaceValidationException);
                return View("WithoutEngagements");
            }
            catch(EmploymentStatusValidationException employmentStatusValidationException)
            {
                TempData["message"] += GetInnerMessage(employmentStatusValidationException);
                return View("WithoutEngagements");
            }
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateEngagement.Request request)
        {
            if(ModelState.IsValid)
            {
                var response = await new CreateEngagement(_ctx).Do(request);
            }

            return RedirectToAction("WithEngagements");
        }
        


        private static string GetInnerMessage(Exception exception) =>
        exception.InnerException.Message;
    }
}
