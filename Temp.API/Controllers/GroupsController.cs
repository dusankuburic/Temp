﻿using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using Temp.Application.Groups;
using Temp.Database;
using Temp.Domain.Models.Groups.Exceptions;
using Microsoft.AspNetCore.Authorization;

namespace Temp.API.Controllers
{
    [Authorize(Roles = "Admin, Moderator")]
    [Route("api/[controller]")]
    [ApiController]
    public class GroupsController : ControllerBase
    {
        private readonly ApplicationDbContext _ctx;

        public GroupsController(ApplicationDbContext ctx)
        {
            _ctx = ctx;
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> Create(CreateGroup.Request request)
        {
            try
            {
                var response = await new CreateGroup(_ctx).Do(request);
                if (response.Status)
                    return NoContent();  

                return BadRequest(response.Message);
            }
            catch (GroupValidationException groupValidationException)
            {
                return BadRequest(GetInnerMessage(groupValidationException));
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetGroup(int id)
        {
            try
            {
                var group = await new GetGroup(_ctx).Do(id);
                return Ok(group);
            }
            catch (GroupValidationException groupValidationException)
            {
                return BadRequest(GetInnerMessage(groupValidationException));
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateGroup(int id, UpdateGroup.Request request)
        {
            try
            {
                var response = await new UpdateGroup(_ctx).Do(id, request);
                if (response.Status)       
                    return NoContent();
                
                return BadRequest(response.Message);
            }
            catch (GroupValidationException groupValidationException)
            {
                return BadRequest(GetInnerMessage(groupValidationException));
            }
        }

        [HttpGet("inner-teams/{id}")]
        public async Task<IActionResult> InnerTeams(int id)
        {
            var response = await new GetInnerTeams(_ctx).Do(id);
            return Ok(response);
        }

        [HttpGet("moderator-groups/{id}")]
        public async Task<IActionResult> GetModeratorGroups(int id)
        {
            var response = await new GetModeratorGroups(_ctx).Do(id);
            return Ok(response);
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("moderator-free-groups/{organizationId}/moderator/{moderatorId}")]
        public async Task<IActionResult> GetModeratorFreeGroups(int organizationId, int moderatorId)
        {
            var response = await new GetModeratorFreeGroups(_ctx).Do(organizationId, moderatorId);
            return Ok(response);
        }
        
        private static string GetInnerMessage(Exception exception) =>
            exception.InnerException.Message;
    }
}