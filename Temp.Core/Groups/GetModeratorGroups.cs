using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Temp.Core.Groups.Service;
using Temp.Database;

namespace Temp.Core.Groups
{
    public class GetModeratorGroups : GroupService
    {
        private readonly ApplicationDbContext _ctx;

        public GetModeratorGroups(ApplicationDbContext ctx)
        {
            _ctx = ctx;
        }

        public Task<IEnumerable<ModeratorGroupViewModel>> Do(int id) =>
        TryCatch(async () =>
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

            ValidateGetModeratorGroupsViewModel(moderatorGroups);

            return moderatorGroups;

        });


        public class ModeratorGroupViewModel
        {
            public int Id { get; set; }
            public string Name { get; set; }
        }
    }
}