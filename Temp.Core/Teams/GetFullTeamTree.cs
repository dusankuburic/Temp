using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Temp.Core.Teams.Service;
using Temp.Database;

namespace Temp.Core.Teams
{
    public class GetFullTeamTree: TeamService
    {
        private readonly ApplicationDbContext _ctx;

        public GetFullTeamTree(ApplicationDbContext ctx)
        {
            _ctx = ctx;
        }

        public Task<TeamTreeViewModel> Do(int id) =>
        TryCatch(async () =>
        {
            var team = await _ctx.Teams
                .Include(x => x.Group)
                .Include(x => x.Group.Organization)
                .Where(x => x.Id == id).FirstOrDefaultAsync();
            
            return new TeamTreeViewModel
            {
                Id = team.Id,
                OrganizationName = team.Group.Organization.Name,
                OrganizationId = team.Group.Organization.Id,
                GroupName = team.Group.Name,
                TeamName = team.Name
            };
        });

        public class TeamTreeViewModel
        {
            public int Id { get; set; }
            public string OrganizationName { get; set; }
            public int OrganizationId { get; set; }
            public string GroupName { get; set; }
            public string TeamName { get; set; }
        } 
    }
}