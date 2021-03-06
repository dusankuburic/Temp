﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using Temp.Application.Organizations;
using Temp.Domain.Models.Organizations.Exceptions;
using System.Threading.Tasks;
using Temp.Database;

namespace Temp.API.Controllers
{
    [Authorize(Roles = "Admin")]
    [Route("api/[controller]")]
    [ApiController]
    public class OrganizationsController : ControllerBase
    {
        private readonly ApplicationDbContext _ctx;

        public OrganizationsController(ApplicationDbContext ctx)
        {
            _ctx = ctx;
        }

        [HttpGet]
        public async Task<IActionResult> GetOrganizations()
        {
            try
            {
                var response = await new GetOrganizations(_ctx).Do();
                return Ok(response);
            }
            catch (OrganizationValidationException organizationValidationException)
            {
                return BadRequest(GetInnerMessage(organizationValidationException));
            }
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateOrganization.Request request)
        {
            try
            {
                var response = await new CreateOrganization(_ctx).Do(request);
                if (response.Status)            
                    return NoContent();
                
                return BadRequest(response.Message);
            }
            catch (OrganizationValidationException organizationValidationException)
            {
                return BadRequest(GetInnerMessage(organizationValidationException));
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetOrganization(int id)
        {
            try
            {
                var response = await new GetOrganization(_ctx).Do(id);
                return Ok(response);
            }
            catch (OrganizationValidationException organizationValidationException)
            {
                return BadRequest(GetInnerMessage(organizationValidationException));
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateOrganization(int id, UpdateOrganization.Request request)
        {
            try
            {
                var response = await new UpdateOrganization(_ctx).Do(id, request);
                if (response.Status)          
                    return NoContent();
                
                return BadRequest(response.Message);
            }
            catch (OrganizationValidationException organizationValidationException)
            {
                return BadRequest(GetInnerMessage(organizationValidationException));
            }
        }

        [HttpGet("inner-groups/{id}")]
        public async Task<IActionResult> InnerGroups(int id)
        {
            var innerGroups = await new GetInnerGroups(_ctx).Do(id);
            return Ok(innerGroups);
        }

        private static string GetInnerMessage(Exception exception) =>
            exception.InnerException.Message;
    }
}