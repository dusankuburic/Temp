using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Temp.Application.EmploymentStatuses;
using Temp.Database;
using Temp.Domain.Models.EmploymentStatuses.Exceptions;

namespace Temp.API.Controllers
{
    [Authorize(Roles = "Admin")]
    [Route("api/[controller]")]
    [ApiController]
    public class EmploymentStatusesController : ControllerBase
    {
        private readonly ApplicationDbContext _ctx;

        public EmploymentStatusesController(ApplicationDbContext ctx)
        {
            _ctx = ctx;
        }

        [HttpGet]
        public ActionResult<IEnumerable<GetEmploymentStatuses.EmploymentStatusViewModel>> GetEmploymentStatuses()
        {
            try
            {
                var response = new GetEmploymentStatuses(_ctx).Do();
                return Ok(response);
            }
            catch (EmploymentStatusValidationException employmentStatusValidationException)
            {
                return BadRequest(GetInnerMessage(employmentStatusValidationException));
            }
        }


        [HttpPost]
        public async Task<IActionResult> Create(CreateEmploymentStatus.Request request)
        {
            try
            {
                var response = await new CreateEmploymentStatus(_ctx).Do(request);
                if (response.Status)
                {
                    return Ok();
                }

                return BadRequest(response.Message);
            }
            catch (EmploymentStatusValidationException employmentStatusValidationException)
            {
                return BadRequest(GetInnerMessage(employmentStatusValidationException));
            }
        }


        [HttpGet("{id}")]
        public IActionResult GetEmploymentStatus(int id)
        {
            try
            {
                var response = new GetEmploymentStatus(_ctx).Do(id);
                return Ok(response);
            }
            catch (EmploymentStatusValidationException employmentStatusValidationException)
            {
                return BadRequest(GetInnerMessage(employmentStatusValidationException));
            }
        }


        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateEmploymentStatus(int id, UpdateEmploymentStatus.Request request)
        {
            try
            {
                var response = await new UpdateEmploymentStatus(_ctx).Do(id, request);
                if (response.Status)
                {
                    return Ok();
                }

                return BadRequest(response.Message);
            }
            catch (EmploymentStatusValidationException employmentStatusValidationException)
            {
                return BadRequest(GetInnerMessage(employmentStatusValidationException));
            }
        }


        private static string GetInnerMessage(Exception exception) =>
            exception.InnerException.Message;
    }
}