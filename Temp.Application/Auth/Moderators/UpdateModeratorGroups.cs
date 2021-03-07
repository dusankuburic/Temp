using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Temp.Database;
using Temp.Domain.Models;

namespace Temp.API.Controllers
{
    public class UpdateModeratorGroups
    {
        private readonly ApplicationDbContext _ctx;

        public UpdateModeratorGroups(ApplicationDbContext ctx)
        {
            _ctx = ctx;
        }


        public async Task<Response> Do(int id, Request request)
        {
            if (!request.Groups.Any())
            {
                foreach (var group in request.Groups)
                {
                    _ctx.ModeratorGroups.Add(new ModeratorGroup
                    {
                        ModeratorId = id,
                        GroupId = group
                    });
                }
            }
            else
            {
                var moderatorGroups = await _ctx.ModeratorGroups.AllAsync(x => x.ModeratorId == id);
                _ctx.Remove(moderatorGroups);
                
                foreach (var group in request.Groups)
                {
                    _ctx.ModeratorGroups.Add(new ModeratorGroup
                    {
                        ModeratorId = id,
                        GroupId = group
                    });
                }
            }

            
            //Validate
            await _ctx.SaveChangesAsync();
            
            return new Response
            {
                Message = $"Groups are assigned",
                Status = true
            };
        }


        public class Request
        {
            [Required] 
            public IEnumerable<int> Groups { get; set; }
        }

        public class Response
        {
            public bool Status { get; set; }
            public string Message { get; set; }
        }
    }
}