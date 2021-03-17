using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Temp.Database;

namespace Temp.Core.Groups
{
    public class GetModeratorFreeGroups
    {
        private readonly ApplicationDbContext _ctx;

        public GetModeratorFreeGroups(ApplicationDbContext ctx)
        {
            _ctx = ctx;
        }

        public async Task<IEnumerable<ModeratorFreeGroupViewModel>> Do(int id, int moderatorId)
        {          
            var moderatorGroups = await _ctx.ModeratorGroups
                .Where(x => x.ModeratorId == moderatorId)
                .Select(x => x.GroupId)
                .ToListAsync();


            var moderatorFreeGroups = await _ctx.Groups
                .Where(x => x.OrganizationId == id)
                .Where(x => !moderatorGroups.Contains(x.Id))
                .Select(x => new ModeratorFreeGroupViewModel
                {
                    Id = x.Id,
                    Name = x.Name
                })
                .ToListAsync();

            return moderatorFreeGroups;
        }
        
        public class ModeratorFreeGroupViewModel
        {
            public int Id { get; set; }
            public string Name { get; set; }
        }
    }
}