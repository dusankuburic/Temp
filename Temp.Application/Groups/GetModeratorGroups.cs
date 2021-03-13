using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Temp.Database;

namespace Temp.Application.Groups
{
    public class GetModeratorGroups
    {
        private readonly ApplicationDbContext _ctx;

        public GetModeratorGroups(ApplicationDbContext ctx)
        {
            _ctx = ctx;
        }


        public async Task<IEnumerable<ModeratorGroupViewModel>> Do(int id)
        {          
            var moderatorGroups = await _ctx.ModeratorGroups
                .Where(x => x.ModeratorId == id)
                .Join(_ctx.Groups,
                    mg => mg.GroupId,
                    group => group.Id,
                    (mg, group) => new ModeratorGroupViewModel
                    {
                        Id = group.Id,
                        Name = group.Name
                    })
                .ToListAsync();

            return moderatorGroups;
        }


        public class ModeratorGroupViewModel
        {
            public int Id { get; set; }
            public string Name { get; set; }
        }
    }
}