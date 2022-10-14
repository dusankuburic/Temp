using Temp.Database;

namespace Temp.Services.Applications.CLI.Query;

public class GetUserApplications
{
    private readonly ApplicationDbContext _ctx;

    public GetUserApplications(ApplicationDbContext ctx) {
        _ctx = ctx;
    }

    public async Task<IEnumerable<ApplicationViewModel>> Do(int id) {
        var applications = await _ctx.Applications
            .Where(x => x.UserId == id)
            .OrderByDescending(x => x.CreatedAt)
            .ThenBy(x => x.Status)
            .Select(x => new ApplicationViewModel
            {
                Id = x.Id,
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
        public string Category { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public bool Status { get; set; }
    }
}

