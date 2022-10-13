using Microsoft.EntityFrameworkCore;
using Temp.Database;

namespace Temp.Services.Applications.CLI.Query;

public class GetTeamApplications
{
    private readonly ApplicationDbContext _ctx;

    public GetTeamApplications(ApplicationDbContext ctx) {
        _ctx = ctx;
    }

    public async Task<IEnumerable<ApplicationViewModel>> Do(int teamId, int moderatorId) {
        var applications = await _ctx.Applications
            .Include(x => x.User)
            .Where(x => x.TeamId == teamId )
            .Where(x => (x.ModeratorId == moderatorId) || (x.Status == false))
            .OrderBy(x => x.Status)
            .Select(x => new ApplicationViewModel
            {
                Id = x.Id,
                TeamId = x.TeamId,
                Username = x.User.Username,
                Category = x.Category,
                CreatedAt = x.CreatedAt,
                Status = x.Status
            })
            .ToListAsync();

        return applications;
    }

    public class ApplicationViewModel
    {
        public int Id { get; set; }
        public int TeamId { get; set; }
        public string Username { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public bool Status { get; set; }
    }
}
