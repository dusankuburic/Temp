using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Temp.Core.Applications.Service;
using Temp.Database;

namespace Temp.Core.Applications;

public class GetTeamApplications : ApplicationService
{
    private readonly ApplicationDbContext _ctx;

    public GetTeamApplications(ApplicationDbContext ctx) {
        _ctx = ctx;
    }

    public Task<IEnumerable<ApplicationViewModel>> Do(int teamId, int moderatorId) =>
        TryCatch(async () => {
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

            ValidateGetTeamApplicationsViewModel(applications);

            return applications;
        });

    public class ApplicationViewModel
    {
        public int Id { get; set; }
        public int TeamId { get; set; }
        public string Username { get; set; }
        public string Category { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool Status { get; set; }
    }
}
