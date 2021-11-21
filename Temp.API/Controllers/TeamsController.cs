using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Temp.Core.Teams;
using Temp.Database;
using Temp.Domain.Models.Teams.Exceptions;

namespace Temp.API.Controllers
{
    [Authorize(Roles = "Admin, User")]
    [Route("api/[controller]")]
    [ApiController]
    public class TeamsController : ControllerBase
    {
        private readonly ApplicationDbContext _ctx;

        public TeamsController(ApplicationDbContext ctx) {
            _ctx = ctx;
        }


        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> Create(CreateTeam.Request request) {
            try {
                var response = await new CreateTeam(_ctx).Do(request);
                if (response.Status)
                    return NoContent();

                return BadRequest(response.Message);
            } catch (TeamValidationException teamValidationException) {
                return BadRequest(GetInnerMessage(teamValidationException));
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetTeam(int id) {
            try {
                var response = await new GetTeam(_ctx).Do(id);
                return Ok(response);
            } catch (TeamValidationException teamValidationException) {
                return BadRequest(GetInnerMessage(teamValidationException));
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("full/{id}")]
        public async Task<IActionResult> GetFullTeam(int id) {
            try {
                var response = await new GetFullTeamTree(_ctx).Do(id);
                return Ok(response);
            } catch (TeamValidationException teamValidationException) {
                return BadRequest(GetInnerMessage(teamValidationException));
            }
        }

        [HttpGet("employee/team/{userId}")]
        public async Task<IActionResult> GetUserTeam(int userId) {
            try {
                var response = await new GetUserTeam(_ctx).Do(userId);
                return Ok(response);
            } catch (TeamValidationException teamValidationException) {
                return BadRequest(GetInnerMessage(teamValidationException));
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTeam(int id, UpdateTeam.Request request) {
            try {
                var response = await new UpdateTeam(_ctx).Do(id, request);
                if (response.Status)
                    return NoContent();

                return BadRequest(response.Message);
            } catch (TeamValidationException teamValidationException) {
                return BadRequest(GetInnerMessage(teamValidationException));
            }
        }

        [HttpPut("change-status/{id}")]
        public async Task<IActionResult> UpdateTeamStatus(int id) {
            var response = await new UpdateTeamStatus(_ctx).Do(id);
            if (response)
                return NoContent();

            return BadRequest();
        }

        private static string GetInnerMessage(Exception exception) =>
            exception.InnerException.Message;
    }
}