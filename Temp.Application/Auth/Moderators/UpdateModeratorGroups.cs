using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Temp.Database;
using Temp.Domain.Models;
using Temp.Application.Auth.Moderators.Service;

namespace Temp.Application.Auth.Moderators
{
    public class UpdateModeratorGroups : ModeratorService
    {
        private readonly ApplicationDbContext _ctx;

        public UpdateModeratorGroups(ApplicationDbContext ctx)
        {
            _ctx = ctx;
        }
        
        public Task<Response> Do(int id, Request request) => 
        TryCatch(async () =>
        {
            if (request.Groups.Count() == 0)
            {
                var mod = await _ctx.ModeratorGroups
                    .Where(x => x.ModeratorId == id)
                    .FirstOrDefaultAsync();
                
                _ctx.Remove(mod);
                
                /*
                foreach (var group in request.Groups)
                {
                    _ctx.ModeratorGroups.Add(new ModeratorGroup
                    {
                        ModeratorId = id,
                        GroupId = group
                    });
                }
                */
            }
            else
            {
                var moderatorGroups = await _ctx.ModeratorGroups
                    .Where(x => x.ModeratorId == id)
                    .ToListAsync();
                
                ValidateModeratorGroups(moderatorGroups);

                _ctx.RemoveRange(moderatorGroups);

                foreach (var group in request.Groups)
                {
                    _ctx.ModeratorGroups.Add(new ModeratorGroup
                    {
                        ModeratorId = id,
                        GroupId = group
                    });
                }
            }
            
            await _ctx.SaveChangesAsync();
        
            return new Response
            {
                Message = $"Groups are assigned",
                Status = true
            };
        });
        
        
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