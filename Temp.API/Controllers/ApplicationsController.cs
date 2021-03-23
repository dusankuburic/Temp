﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using Temp.Core.Applications;
using Temp.Database;
using Temp.Domain.Models.Applications.Exceptions;

namespace Temp.API.Controllers
{
    [Authorize(Roles = "Moderator, User, Admin")]
    [Route("api/[controller]")]
    [ApiController]
    public class ApplicationsController : ControllerBase
    {
        private readonly ApplicationDbContext _ctx;

        public ApplicationsController(ApplicationDbContext ctx)
        {
            _ctx = ctx;
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateApplication.Request request)
        {
            try
            {
                var response = await new CreateApplication(_ctx).Do(request);
                if (response.Status)
                    return NoContent();

                return BadRequest("Error");
            }
            catch(ApplicationValidationException applicationValidationException)
            {
                return BadRequest(GetInnerMessage(applicationValidationException));
            }

        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetApplication(int id)
        {
            var response = await new GetApplication(_ctx).Do(id);
            return Ok(response);
        }

        [HttpGet("team/{id}")]
        public async Task<IActionResult> GetTeamApplications(int id)
        {
            var response = await new GetTeamApplications(_ctx).Do(id);
            return Ok(response);
        }

        [HttpGet("user/{id}")]
        public async Task<IActionResult> GetUserApplications(int id)
        {
            var response = await new GetUserApplications(_ctx).Do(id);
            return Ok(response);
        }

        private static string GetInnerMessage(Exception exception) =>
            exception.InnerException.Message;
    }
}