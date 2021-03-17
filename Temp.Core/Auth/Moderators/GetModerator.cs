using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Temp.Database;

namespace Temp.Core.Auth.Moderators
{
    public class GetModerator
    {
        private readonly ApplicationDbContext _ctx;

        public GetModerator(ApplicationDbContext ctx)
        {
            _ctx = ctx;
        }

        public async Task<ModeratorViewModel> Do(int id)
        {
            var moderator = await _ctx.Moderators
                .Where(x => x.EmployeeId == id)
                .Select(x => new ModeratorViewModel
                {
                    Id = x.Id,
                    Username = x.Username
                })
                .FirstOrDefaultAsync();

            return moderator;
        }


        public class ModeratorViewModel
        {
            public int Id { get; set; }
            public string Username { get; set; }
        }
    }
}