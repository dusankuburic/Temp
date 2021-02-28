using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Temp.Application.Groups;
using Temp.Application.Teams;
using Temp.Database;
using Temp.Domain.Models.Groups.Exceptions;
using Temp.Domain.Models.Teams.Exceptions;

namespace Temp.API.Controllers
{
    [Authorize(Roles = "Admin")]
    [Route("api/[controller]")]
    [ApiController]
    public class TeamsController : ControllerBase
    {
        private readonly ApplicationDbContext _ctx;

        public TeamsController(ApplicationDbContext ctx)
        {
            _ctx = ctx;
        }


        [HttpPost]
        public async Task<IActionResult> Create(CreateTeam.Request request)
        {
            try
            {
                var response = await new CreateTeam(_ctx).Do(request);
                if (response.Status)
                {
                    return Ok(response);
                }

                return BadRequest(response.Message);
            }
            catch (TeamValidationException teamValidationException)
            {
                return BadRequest(GetInnerMessage(teamValidationException));
            }
        }

        [HttpGet("{id}")]
        public IActionResult GetTeam(int id)
        {
            try
            {
                var response = new GetTeam(_ctx).Do(id);
                return Ok(response);
            }
            catch (TeamValidationException teamValidationException)
            {
                return BadRequest(GetInnerMessage(teamValidationException));
            }
        }


        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTeam(int id, UpdateTeam.Request request)
        {
            try
            {
                var response = await new UpdateTeam(_ctx).Do(id, request);
                if (response.Status)
                {
                    return Ok(response);
                }

                return BadRequest(response.Message);
            }
            catch (TeamValidationException teamValidationException)
            {
                return BadRequest(GetInnerMessage(teamValidationException));
            }
        }

        private static string GetInnerMessage(Exception exception) =>
            exception.InnerException.Message;
    }
}