using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Temp.Core.Applications.Service;
using Temp.Database;

namespace Temp.Core.Applications;

public class GetUserApplications : ApplicationService
{
    private readonly ApplicationDbContext _ctx;

    public GetUserApplications(ApplicationDbContext ctx) {
        _ctx = ctx;
    }

    public Task<IEnumerable<ApplicationViewModel>> Do(int id) =>
        TryCatch(async () => {
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

            ValidateGetUserApplicationsViewModel(applications);

            return applications;
        });

    public class ApplicationViewModel
    {
        public int Id { get; set; }
        public string Category { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool Status { get; set; }
    }
}
